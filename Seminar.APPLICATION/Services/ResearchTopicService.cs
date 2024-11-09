using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.CORE.Utils;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;
namespace Seminar.APPLICATION.Services;

public class ResearchTopicService : IResearchTopicService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    public ResearchTopicService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
    }

    // Research Topic
    public async Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByCompetitionIdAsync(int competitionId, int index, int pageSize, string nameTopicSearch, int disciplineId)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(competitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
        if (competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not allowed to access this competition!");
        }
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }
        IQueryable<ResearchTopic> query = _unitOfWork.GetRepository<ResearchTopic>().Entities
        .Where(r => r.DeletedAt == null && r.CompetitionId == competitionId)
        .OrderByDescending(r => r.CreatedAt);

        if (!string.IsNullOrEmpty(nameTopicSearch))
        {
            query = query.Where(r => r.NameTopic.Contains(nameTopicSearch));
        }
        if (disciplineId > 0)
        {
            query = query.Where(r => r.DisciplineId == disciplineId);
        }
        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ResearchTopicVM>(new List<ResearchTopicVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        List<ResearchTopicVM> responeItems = _mapper.Map<List<ResearchTopicVM>>(resultQuery);
        foreach (ResearchTopicVM researchTopicVM in responeItems)
        {
            List<ResearchTopicAuthorVM> coAuthors = await GetAuthorByResearchTopicIdAsync(researchTopicVM.Id);
            researchTopicVM.CoAuthors = coAuthors;
        }
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ResearchTopicVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByAuthorIdAsync(string roleName, int index, int pageSize, string nameTopicSearch)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        if (roleName != CLAIMS_VALUES.ROLE_TYPE.AUTHOR && roleName != CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR && roleName != "")
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid role name!");
        }
        IQueryable<ResearchTopic> query = _unitOfWork.GetRepository<ResearchTopic>().Entities
            .Include(r => r.Author_ResearchTopics)
            .Where(r => r.DeletedAt == null && r.Author_ResearchTopics.Any(a => a.AuthorId == author.Id))
            .OrderByDescending(r => r.CreatedAt);

        if (!string.IsNullOrEmpty(nameTopicSearch))
        {
            query = query.Where(r => r.NameTopic.Contains(nameTopicSearch));
        }
        if (!string.IsNullOrEmpty(roleName))
        {
            query = query.Where(r => r.Author_ResearchTopics.Any(a => a.RoleName == roleName));
        }
        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ResearchTopicVM>(new List<ResearchTopicVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        List<ResearchTopicVM> responeItems = _mapper.Map<List<ResearchTopicVM>>(resultQuery);
        foreach (ResearchTopicVM researchTopicVM in responeItems)
        {
            List<ResearchTopicAuthorVM> coAuthors = await GetAuthorByResearchTopicIdAsync(researchTopicVM.Id);
            researchTopicVM.CoAuthors = coAuthors;
        }
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ResearchTopicVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
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
                    // Kiểm tra article có tồn tại không và article có phải của author không
                    // ... existing code ...
                    if (createResearchTopicDto.ArticleId == 0)
                    {
                        createResearchTopicDto.ArticleId = null;
                    }
                    else
                    {
                        Article? article = await _unitOfWork.GetRepository<Article>().Entities.FirstOrDefaultAsync(a => a.Id == createResearchTopicDto.ArticleId)
                            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
                        Author_Article? author_Article = await _unitOfWork.GetRepository<Author_Article>().Entities.FirstOrDefaultAsync(a => a.ArticleId == createResearchTopicDto.ArticleId && a.AuthorId == mainAuthor.Id) ??
                            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author article not found!");
                        if (author_Article.AuthorId != mainAuthor.Id)
                        {
                            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Article is not owned by the author!");
                        }
                    }
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
                        RoleName = CLAIMS_VALUES.ROLE_TYPE.AUTHOR
                    }
                    };

                    // Xử lý co-authors
                    if (createResearchTopicDto.CoAuthors != null && createResearchTopicDto.CoAuthors.Count > 0)
                    {
                        // Kiểm tra email trùng lặp
                        await ValidateCoAuthorEmailsAsync(createResearchTopicDto.CoAuthors);

                        foreach (CoAuthorDto coAuthorDto in createResearchTopicDto.CoAuthors)
                        {
                            int coAuthorId = await CreateOrUpdateCoAuthorAsync(coAuthorDto, competition, researchTopic);

                            author_ResearchTopics.Add(new Author_ResearchTopic
                            {
                                AuthorId = coAuthorId,
                                ResearchTopicId = researchTopic.Id,
                                RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR
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
    private async Task ValidateCoAuthorEmailsAsync(IEnumerable<CoAuthorDto> coAuthors)
    {
        var processedEmails = new HashSet<string>();
        foreach (CoAuthorDto coAuthor in coAuthors)
        {
            // Kiểm tra email trùng lặp trong danh sách
            if (!processedEmails.Add(coAuthor.Email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Co-author email is duplicated!");
            }

            // Kiểm tra email có tồn tại trong hệ thống không và vai trò của tài khoản
            Account? account = await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(a => a.Email == coAuthor.Email);
            if (account != null)
            {
                if (account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.REVIEWER || account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Email is already associated with system!");
                }
            }
        }
    }
    private async Task<int> CreateOrUpdateCoAuthorAsync(CoAuthorDto coAuthorDto, Competition competition, ResearchTopic researchTopic)
    {
        Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);
        int coAuthorId;

        if (existingCoAuthor == null)
        {
            // Tạo mới co-author
            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
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

        return coAuthorId;
    }
    public async Task UpdateResearchTopicAsync(int researchTopicId, UpdateResearchTopicDto updateResearchTopicDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(a => a.ResearchTopicId == researchTopicId && a.AuthorId == author.Id && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author research topic not found or not an author!");
        if (researchTopic.IsAcceptanceApproved == true || researchTopic.IsReviewAcceptance == true)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "The research topic has been accepted or confirmed and cannot be edited");
        }
        var strategy = _unitOfWork.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    ResearchTopic updateResearchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ??
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
                    _mapper.Map(updateResearchTopicDto, updateResearchTopic);
                    updateResearchTopic.UpdatedAt = DateTime.Now;
                    updateResearchTopic.ArticleId = updateResearchTopicDto.ArticleId == 0 ? null : updateResearchTopicDto.ArticleId;
                    if (updateResearchTopicDto.CoAuthors != null && updateResearchTopicDto.CoAuthors.Count > 0)
                    {
                        // Lấy co-authors hiện tại
                        List<Author_ResearchTopic> existingCoAuthors = await _unitOfWork.GetRepository<Author_ResearchTopic>()
                        .Entities.Where(a => a.ResearchTopicId == researchTopicId && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR)
                        .ToListAsync();
                        List<Author_ResearchTopic> author_ResearchTopics = new List<Author_ResearchTopic>();
                        foreach (CoAuthorDto coAuthorDto in updateResearchTopicDto.CoAuthors)
                        {
                            await ValidateCoAuthorEmailsAsync(updateResearchTopicDto.CoAuthors);
                            // Kiểm tra xem co-author đã tồn tại trong research topic chưa
                            var existingAuthorResearchTopic = existingCoAuthors
                                .FirstOrDefault(x => x.Author.Email?.ToLower() == coAuthorDto.Email.ToLower());
                            if (existingAuthorResearchTopic != null)
                            {
                                continue;
                            }
                            Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>()
                            .Entities.FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);
                            Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(researchTopic.CompetitionId) ??
                            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
                            int coAuthorId = existingCoAuthor?.Id ?? await CreateOrUpdateCoAuthorAsync(coAuthorDto, competition, researchTopic);
                            author_ResearchTopics.Add(new Author_ResearchTopic
                            {
                                AuthorId = coAuthorId,
                                ResearchTopicId = researchTopic.Id,
                                RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR
                            });
                        }
                        await _unitOfWork.GetRepository<Author_ResearchTopic>().InsertRangeAsync(author_ResearchTopics);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    await _unitOfWork.GetRepository<ResearchTopic>().UpdateAsync(updateResearchTopic);
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
    public async Task<ResearchTopicVM> GetResearchTopicByIdAsync(int id)
    {
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        ResearchTopicVM researchTopicVM = _mapper.Map<ResearchTopicVM>(researchTopic);
        List<ResearchTopicAuthorVM> coAuthors = await GetAuthorByResearchTopicIdAsync(id);
        researchTopicVM.CoAuthors = coAuthors;
        return researchTopicVM;
    }
    public async Task<List<ResearchTopicAuthorVM>> GetAuthorByResearchTopicIdAsync(int id)
    {
        List<Author_ResearchTopic> author_ResearchTopics = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.Where(a => a.ResearchTopicId == id && a.DeletedAt == null).ToListAsync();
        List<ResearchTopicAuthorVM> researchTopicAuthorVMs = author_ResearchTopics.Select(a => new ResearchTopicAuthorVM
        {
            Id = a.Author.Id,
            AccountId = a.Author.AccountId ?? 0,
            Name = a.Author.Name ?? "Unknown",
            Email = a.Author.Email ?? "No email",
            NumberPhone = a.Author.NumberPhone ?? "No phone",
            DateOfBirth = a.Author.DateOfBirth ?? DateTime.MinValue,
            Sex = a.Author.Sex ?? "Unknown",
            FacultyId = a.Author.FacultyId,
            FacultyName = a.Author.Faculty?.FacultyName ?? "Unknown Faculty",
            InternalCode = a.Author.InternalCode ?? "No code",
            RoleName = a.RoleName ?? "Unknown Role"
        }).ToList();
        return researchTopicAuthorVMs;
    }
    public async Task UpdateIsAcceptanceApprovedAsync(UpdateIsAcceptanceApprovedDto updateIsAcceptanceApprovedDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        Competition competition = await _unitOfWork.GetRepository<Competition>().Entities.FirstOrDefaultAsync(c => c.Id == updateIsAcceptanceApprovedDto.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
        if (competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not allowed to access this competition!");
        }
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(updateIsAcceptanceApprovedDto.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        researchTopic.IsAcceptanceApproved = updateIsAcceptanceApprovedDto.IsAcceptanceApproved;
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateIsReviewAcceptanceAsync(UpdateIsReviewAcceptanceDto updateIsReviewAcceptanceDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(updateIsReviewAcceptanceDto.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        if (researchTopic.Competitions.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not allowed to access this research topic!");
        }
        researchTopic.IsReviewAcceptance = updateIsReviewAcceptanceDto.IsReviewAcceptance;
        await _unitOfWork.SaveChangesAsync();
    }
    // History Research Topic
    public async Task CreateNewVersionResearchTopicAsync(CreateHistoryResearchTopicDto createHistoryResearchTopicDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(createHistoryResearchTopicDto.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(researchTopic.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
        DateTime now = DateTime.Now;
        if (competition.DateEnd < now)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Competition has expired!");
        }
        if (researchTopic.IsAcceptanceApproved == true || researchTopic.IsReviewAcceptance == true)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "The research topic has been accepted or confirmed and cannot be edited");
        }
        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(a => a.AuthorId == author.Id && a.ResearchTopicId == createHistoryResearchTopicDto.ResearchTopicId && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author research topic not found or not an author!");
        History_Update_ResearchTopic history_Update_ResearchTopic = _mapper.Map<History_Update_ResearchTopic>(createHistoryResearchTopicDto);
        history_Update_ResearchTopic.DateUpdate = DateTime.Now;
        await _unitOfWork.GetRepository<History_Update_ResearchTopic>().InsertAsync(history_Update_ResearchTopic);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<List<HistoryUpdateResearchTopicVM>> GetAllHistoryResearchTopicByResearchTopicIdAsync(int researchTopicId)
    {
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        List<History_Update_ResearchTopic> history_Update_ResearchTopics = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().Entities.Where(h => h.ResearchTopicId == researchTopicId).ToListAsync();
        List<HistoryUpdateResearchTopicVM> historyUpdateResearchTopicVMs = _mapper
        .Map<List<HistoryUpdateResearchTopicVM>>(history_Update_ResearchTopics);
        return historyUpdateResearchTopicVMs;
    }

}