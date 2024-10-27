using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.CORE.Utils;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;
namespace Seminar.APPLICATION.Services;

public class ResearchTopicService : IResearchTopicService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFirebaseService _firebaseService;
    private readonly IEmailService _emailService;
    private readonly IAuthorService _authorService;
    public ResearchTopicService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService, IEmailService emailService, IAuthorService authorService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _firebaseService = firebaseService;
        _emailService = emailService;
        _authorService = authorService;
    }
    public async Task CreateResearchTopicAsync(CreateResearchTopicDto createResearchTopicDto)
    {
        var strategy = _unitOfWork.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Lấy main author
                    int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
                    Author mainAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");

                    // Kiểm tra competition còn thời gian nộp đề tài không
                    Competition competition = await _unitOfWork.GetRepository<Competition>().Entities.FirstOrDefaultAsync(c => c.Id == createResearchTopicDto.CompetitionId) ??
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
                    if (competition.DateEndSubmit < DateTime.Now || competition.DateStart > DateTime.Now)
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Competition has expired or not started yet!");
                    }

                    // Kiểm tra author có đăng ký đề tài thành công không
                    RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().Entities.FirstOrDefaultAsync(r => r.AuthorId == mainAuthor.Id && r.CompetitionId == createResearchTopicDto.CompetitionId && r.IsAccepted == (int)RegistrationFormEnum.Approved) ??
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author has not successfully registered a topic for this competition!");

                    // Thêm đề tài
                    ResearchTopic researchTopic = _mapper.Map<ResearchTopic>(createResearchTopicDto);
                    researchTopic.DateUpLoad = DateTime.Now;
                    researchTopic.IsAcceptanceApproved = false;
                    researchTopic.IsReviewAcceptance = false;
                    researchTopic.ArticleId = createResearchTopicDto.ArticleId == 0 ? null : createResearchTopicDto.ArticleId;
                    await _unitOfWork.GetRepository<ResearchTopic>().InsertAsync(researchTopic);
                    await _unitOfWork.SaveChangesAsync();

                    // Thêm main author vào đề tài
                    List<Author_ResearchTopic> author_ResearchTopics = new List<Author_ResearchTopic>
                    {
                    new Author_ResearchTopic
                    {
                        AuthorId = mainAuthor.Id,
                        ResearchTopicId = researchTopic.Id,
                        RoleName = "author"
                    }
                    };

                    // Xử lý co-authors
                    if (createResearchTopicDto.CoAuthors != null && createResearchTopicDto.CoAuthors.Count > 0)
                    {
                        // Kiểm tra email trùng lặp
                        var processedEmails = new HashSet<string>();
                        foreach (CoAuthorDto coAuthorDto in createResearchTopicDto.CoAuthors)
                        {
                            if (!processedEmails.Add(coAuthorDto.Email))
                            {
                                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Co-author email is duplicated!");
                            }

                            Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);
                            int coAuthorId;

                            if (existingCoAuthor == null)
                            {
                                // Tạo mới co-author
                                Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == "author") ??
                                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found!");

                                FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
                                Account account = new Account
                                {
                                    Email = coAuthorDto.Email,
                                    Password = passwordHasher.HashPassword(new Account(), "Huit@1245"),
                                    RoleId = role.Id,
                                    IsSuspended = false,
                                };
                                await _unitOfWork.GetRepository<Account>().InsertAsync(account);
                                await _unitOfWork.SaveChangesAsync();

                                Author newCoAuthor = new Author
                                {
                                    AccountId = account.Id,
                                    Name = coAuthorDto.Name,
                                    Email = account.Email,
                                    NumberPhone = coAuthorDto.NumberPhone,
                                    DateOfBirth = coAuthorDto.DateOfBirth,
                                    Sex = coAuthorDto.Sex
                                };
                                await _unitOfWork.GetRepository<Author>().InsertAsync(newCoAuthor);
                                await _unitOfWork.SaveChangesAsync();

                                await _emailService.SendMemberAccountInfoEmail(coAuthorDto, competition.CompetitionName);
                                coAuthorId = newCoAuthor.Id;
                            }
                            else
                            {
                                coAuthorId = existingCoAuthor.Id;
                                Author_ResearchTopic? existingCoAuthorResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities
                                    .FirstOrDefaultAsync(a => a.AuthorId == coAuthorId && a.ResearchTopicId == researchTopic.Id);
                                if (existingCoAuthorResearchTopic != null)
                                {
                                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Co-author is existed!");
                                }
                            }

                            author_ResearchTopics.Add(new Author_ResearchTopic
                            {
                                AuthorId = coAuthorId,
                                ResearchTopicId = researchTopic.Id,
                                RoleName = "co-author"
                            });
                        }
                    }

                    await _unitOfWork.GetRepository<Author_ResearchTopic>().InsertRangeAsync(author_ResearchTopics);
                    await _unitOfWork.SaveChangesAsync();
                    // Insert history article version 1
                    History_Update_ResearchTopic history_Update_ResearchTopic = new History_Update_ResearchTopic
                    {
                        ResearchTopicId = researchTopic.Id,
                        DateUpdate = DateTime.Now,
                        Summary = createResearchTopicDto.Summary,
                        NewProductFilePath = researchTopic.ProductFilePath,
                        NewReportFilePath = researchTopic.ReportFilePath
                    };
                    await _unitOfWork.GetRepository<History_Update_ResearchTopic>().InsertAsync(history_Update_ResearchTopic);
                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollBackAsync();
                    throw;
                }
            }
        });
    }
}
