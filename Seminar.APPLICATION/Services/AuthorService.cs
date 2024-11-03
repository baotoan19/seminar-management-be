using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.CORE.Utils;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthorService> logger, IHttpContextAccessor httpContextAccessor, IAccountService accountService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _emailService = emailService;
        }

        public async Task<Author> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            Author? existsAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == createAuthorDto.AccountId);
            if (existsAuthor != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Author is existed!");
            }

            Author author = _mapper.Map<Author>(createAuthorDto);
            await _unitOfWork.GetRepository<Author>().InsertAsync(author);
            await _unitOfWork.SaveChangesAsync();
            return author;
        }

        public async Task<AuthorVM> GetAuthorInforAsync(int id)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == id) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
            AuthorVM authorVM = new AuthorVM
            {
                Id = author.Id,
                Name = author.Name ?? "Unknown",
                Email = author.Email ?? "Unknown",
                AccountId = author.AccountId ?? 0,
                FacultyId = author.FacultyId ?? 0,
                FacultyName = author.Faculty?.FacultyName ?? "Unknown",
                DateOfBirth = author.DateOfBirth ?? DateTime.MinValue,
                Sex = author.Sex ?? "Unknown",
                NumberPhone = author.NumberPhone ?? "Unknown",
                InternalCode = author.InternalCode ?? "Unknown",
            };
            return authorVM;
        }

        public async Task UpdateAuthorAsync(int accountId, UpdateAuthorDto updateAuthorDto)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.Include(a => a.Account).Include(a => a.Faculty).FirstOrDefaultAsync(a => a.AccountId == accountId) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
            _mapper.Map(updateAuthorDto, author);
            if (await _accountService.IsEmailUniqueAsync(updateAuthorDto.Email, accountId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Email is existed!");
            }
            author.Account.Email = updateAuthorDto.Email;
            author.Account.UpdatedAt = DateTime.Now;
            author.UpdatedAt = DateTime.Now;
            await _unitOfWork.GetRepository<Author>().UpdateAsync(author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateCoAuthorAsync(int articleId, CreateCoAuthorDto createCoAuthorDto)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
                        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
                        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(articleId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
                        //Kiểm tra article đã được publish chưa
                        if (article.IsAcceptedForPublication == false)
                        {
                            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Article is not accepted for publication!");
                        }
                        // Kiểm tra xem user đã là author của article hay chưa
                        Author_Article? existingAuthorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
                        .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.AuthorId == author.Id && aa.RoleName == "author");
                        if (existingAuthorArticle == null)
                        {
                            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not authorized to create co-author for this article!");
                        }
                        //Kiểm tra email của co-author đã tồn tại chưa
                        FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
                        int coAuthorId;
                        foreach (var coAuthorDto in createCoAuthorDto.CoAuthors)
                        {
                            Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>().Entities
                            .FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);

                            if (existingCoAuthor == null)
                            {
                                Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == "author") ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found!");
                                Account account = new Account
                                {
                                    Email = coAuthorDto.Email,
                                    Password = passwordHasher.HashPassword(new Account(), "Huit@1245"),
                                    RoleId = role.Id,
                                    IsSuspended = false,
                                };
                                await _unitOfWork.GetRepository<Account>().InsertAsync(account);
                                await _unitOfWork.SaveChangesAsync();
                                Author newAuthor = new Author
                                {
                                    AccountId = account.Id,
                                    Name = coAuthorDto.Name,
                                    Email = account.Email,
                                    NumberPhone = coAuthorDto.NumberPhone,
                                    DateOfBirth = coAuthorDto.DateOfBirth,
                                    Sex = coAuthorDto.Sex
                                };
                                await _unitOfWork.GetRepository<Author>().InsertAsync(newAuthor);
                                await _unitOfWork.SaveChangesAsync();
                                coAuthorId = newAuthor.Id;
                                await _emailService.SendCoAuthorAccountInfoEmail(coAuthorDto);
                            }
                            else
                            {
                                coAuthorId = existingCoAuthor.Id;
                                // Kiểm tra co-author đã tồn tại chưa
                                Author_Article? existingCoAuthorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
                                .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.AuthorId == coAuthorId);
                                if (existingCoAuthorArticle != null)
                                {
                                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, $"Co-author with email {coAuthorDto.Email} is existed!");
                                }
                            }

                            Author_Article authorArticle = new Author_Article
                            {
                                ArticleId = articleId,
                                AuthorId = coAuthorId,
                                RoleName = "co-author"
                            };
                            await _unitOfWork.GetRepository<Author_Article>().InsertAsync(authorArticle);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        await _unitOfWork.CommitTransactionAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating co-author");
                        await _unitOfWork.RollBackAsync();
                        throw;
                    }
                }
            });
        }

        public async Task CreateMemberAsync(int researchTopicId, CreateCoAuthorDto createCoAuthorDto)
        {
            int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
            Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
            ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
            // Kiểm tra xem user đã là author của research topic hay chưa
            Author_ResearchTopic? existingAuthorResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities
            .FirstOrDefaultAsync(a => a.ResearchTopicId == researchTopicId && a.AuthorId == author.Id && a.RoleName == "author");
            if (existingAuthorResearchTopic == null)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not authorized to create a member for this research topic!");
            }
            //Kiểm tra email của co-author đã tồn tại chưa
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            int coAuthorId;
            foreach (var coAuthorDto in createCoAuthorDto.CoAuthors)
            {
                Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>().Entities
                .FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);
                if (existingCoAuthor == null)
                {
                    Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == "author") ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found!");
                    Account account = new Account
                    {
                        Email = coAuthorDto.Email,
                        Password = passwordHasher.HashPassword(new Account(), "Huit@1245"),
                        RoleId = role.Id,
                        IsSuspended = false,
                    };
                    await _unitOfWork.GetRepository<Account>().InsertAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                    Author newAuthor = new Author
                    {
                        AccountId = account.Id,
                        Name = coAuthorDto.Name,
                        Email = account.Email,
                        NumberPhone = coAuthorDto.NumberPhone,
                        DateOfBirth = coAuthorDto.DateOfBirth,
                        Sex = coAuthorDto.Sex
                    };
                    await _unitOfWork.GetRepository<Author>().InsertAsync(newAuthor);
                    await _unitOfWork.SaveChangesAsync();
                    coAuthorId = newAuthor.Id;

                    await _emailService.SendMemberAccountInfoEmail(coAuthorDto, researchTopic.NameTopic);
                }
                else
                {
                    coAuthorId = existingCoAuthor.Id;
                    // Kiểm tra co-author đã tồn tại chưa
                    Author_ResearchTopic? existingCoAuthorResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities
                    .FirstOrDefaultAsync(aa => aa.ResearchTopicId == researchTopicId && aa.AuthorId == coAuthorId);
                    if (existingCoAuthorResearchTopic != null)
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, $"Co-author with email {coAuthorDto.Email} is existed!");
                    }
                }

                Author_ResearchTopic authorResearchTopic = new Author_ResearchTopic
                {
                    ResearchTopicId = researchTopicId,
                    AuthorId = coAuthorId,
                    RoleName = "co-author"
                };
                await _unitOfWork.GetRepository<Author_ResearchTopic>().InsertAsync(authorResearchTopic);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteCoAuthorAsync(int articleId, int coAuthorId)
        {
            int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
            // Kiểm tra xem người đăng nhập có phải là tác giả không
            Author author = await _unitOfWork.GetRepository<Author>().Entities
                .FirstOrDefaultAsync(a => a.AccountId == userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
            // Kiểm tra xem người đăng nhập có phải là tác giả chính của bài viết không
            Author_Article? mainAuthor = await _unitOfWork.GetRepository<Author_Article>().Entities
                .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.Author.AccountId == userId && aa.RoleName == "author");
            if (mainAuthor == null)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not authorized to delete co-author for this article!");
            }
            // Tìm Author_Article của tác giả phụ cần xóa
            Author_Article? coAuthorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
                .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.AuthorId == coAuthorId);
            if (coAuthorArticle == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Co-author not found for this article!");
            }
            // Xóa Author_Article của tác giả phụ
            coAuthorArticle.DeletedAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}