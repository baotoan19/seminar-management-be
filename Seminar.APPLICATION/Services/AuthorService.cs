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
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.CORE.Utils;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
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
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Tác giả đã tồn tại!");
            }

            Author author = _mapper.Map<Author>(createAuthorDto);
            await _unitOfWork.GetRepository<Author>().InsertAsync(author);
            await _unitOfWork.SaveChangesAsync();
            return author;
        }
        public async Task<AuthorVM> GetAuthorInforAsync(int id)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == id) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
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
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
            _mapper.Map(updateAuthorDto, author);
            if (await _accountService.IsEmailUniqueAsync(updateAuthorDto.Email, accountId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Email đã tồn tại!");
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
                        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId)
                            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");

                        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(articleId)
                            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");

                        // Kiểm tra xem article đã được chấp nhận để xuất bản chưa
                        if (article.AcceptedForPublicationStatus == (int)AcceptanceApprovedStatusEnum.Approved)
                        {
                            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Bài báo đã được xuất bản, không thể thêm đồng tác giả!");
                        }

                        // Kiểm tra quyền tác giả của user trên article
                        Author_Article? existingAuthorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
                            .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.AuthorId == author.Id && aa.RoleName == "author");
                        if (existingAuthorArticle == null)
                        {
                            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không có quyền tạo đồng tác giả cho bài báo này!");
                        }

                        FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
                        List<CoAuthorDto> emailsToSend = new List<CoAuthorDto>();

                        foreach (var coAuthorDto in createCoAuthorDto.CoAuthors)
                        {
                            await ValidateCoAuthorEmailsAsync(createCoAuthorDto.CoAuthors);
                            Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>().Entities
                                .FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);
                            int coAuthorId;

                            if (existingCoAuthor == null)
                            {
                                Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == "author")
                                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Vai trò không tồn tại!");

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
                                emailsToSend.Add(coAuthorDto); // Thêm vào danh sách email cần gửi
                            }
                            else
                            {
                                coAuthorId = existingCoAuthor.Id;
                                Author_Article? existingCoAuthorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
                                    .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.AuthorId == coAuthorId);
                                if (existingCoAuthorArticle != null)
                                {
                                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, $"Đồng tác giả với email {coAuthorDto.Email} đã tồn tại!");
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

                        // Commit transaction
                        await _unitOfWork.CommitTransactionAsync();

                        // Gửi email sau khi transaction đã commit thành công
                        foreach (var coAuthor in emailsToSend)
                        {
                            await _emailService.SendCoAuthorAccountInfoEmail(coAuthor);
                        }
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollBackAsync();
                        throw;
                    }
                }
            });
        }
        private async Task ValidateCoAuthorEmailsAsync(List<CoAuthorDto> coAuthors)
        {
            var processedEmails = new HashSet<string>();
            foreach (CoAuthorDto coAuthor in coAuthors)
            {
                // Kiểm tra email trùng lặp trong danh sách
                if (!processedEmails.Add(coAuthor.Email))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Email đồng tác giả trùng lặp!");
                }
                // Kiểm tra email có tồn tại trong hệ thống không và vai trò của tài khoản
                Account? account = await _unitOfWork.GetRepository<Account>().Entities
                    .FirstOrDefaultAsync(a => a.Email == coAuthor.Email);
                if (account != null)
                {
                    if (account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.REVIEWER || account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Email đã được liên kết với hệ thống!");
                    }
                }
            }
        }
        public async Task CreateMemberAsync(int researchTopicId, CreateCoAuthorDto createCoAuthorDto)
        {
            int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
            Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
            ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Chủ đề nghiên cứu không tồn tại!");

            Author_ResearchTopic? existingAuthorResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities
                .FirstOrDefaultAsync(a => a.ResearchTopicId == researchTopicId && a.AuthorId == author.Id && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR);
            if (existingAuthorResearchTopic == null)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không có quyền tạo thành viên cho chủ đề nghiên cứu này!");
            }

            var strategy = _unitOfWork.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
                        List<CoAuthorDto> emailsToSend = new List<CoAuthorDto>();

                        foreach (var coAuthorDto in createCoAuthorDto.CoAuthors)
                        {
                            await ValidateCoAuthorEmailsAsync(createCoAuthorDto.CoAuthors);

                            Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>().Entities
                                .FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);
                            int coAuthorId;

                            if (existingCoAuthor == null)
                            {
                                Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR)
                                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Vai trò không tồn tại!");

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
                                emailsToSend.Add(coAuthorDto); // Thêm vào danh sách email cần gửi
                            }
                            else
                            {
                                coAuthorId = existingCoAuthor.Id;
                                Author_ResearchTopic? existingCoAuthorResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities
                                    .FirstOrDefaultAsync(aa => aa.ResearchTopicId == researchTopicId && aa.AuthorId == coAuthorId);
                                if (existingCoAuthorResearchTopic != null)
                                {
                                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, $"Đồng tác giả với email {coAuthorDto.Email} đã tồn tại!");
                                }
                            }

                            Author_ResearchTopic authorResearchTopic = new Author_ResearchTopic
                            {
                                ResearchTopicId = researchTopicId,
                                AuthorId = coAuthorId,
                                RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR
                            };
                            await _unitOfWork.GetRepository<Author_ResearchTopic>().InsertAsync(authorResearchTopic);
                            await _unitOfWork.SaveChangesAsync();
                        }

                        await _unitOfWork.CommitTransactionAsync();

                        // Gửi email sau khi tất cả đã được lưu thành công
                        foreach (var coAuthor in emailsToSend)
                        {
                            await _emailService.SendMemberAccountInfoEmail(coAuthor, researchTopic.NameTopic);
                        }
                    }
                    catch
                    {
                        await _unitOfWork.RollBackAsync();
                        throw;
                    }
                }
            });
        }
        public async Task DeleteCoAuthorAsync(int articleId, int coAuthorId)
        {
            int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
            // Kiểm tra xem người đăng nhập có phải là tác giả không
            Author author = await _unitOfWork.GetRepository<Author>().Entities
                .FirstOrDefaultAsync(a => a.AccountId == userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
            // Kiểm tra xem người đăng nhập có phải là tác giả chính của bài viết không
            Author_Article? mainAuthor = await _unitOfWork.GetRepository<Author_Article>().Entities
                .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.Author.AccountId == userId && aa.RoleName == "author");
            if (mainAuthor == null)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không có quyền xóa đồng tác giả cho bài báo này!");
            }
            // Tìm Author_Article của tác giả phụ cần xóa
            Author_Article? coAuthorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
                .FirstOrDefaultAsync(aa => aa.ArticleId == articleId && aa.AuthorId == coAuthorId);
            if (coAuthorArticle == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đồng tác giả không tồn tại cho bài báo này!");
            }
            // Xóa Author_Article của tác giả phụ
            coAuthorArticle.DeletedAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteMemberAsync(int researchTopicId, int memberId)
        {
            int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
            Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
            ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Chủ đề nghiên cứu không tồn tại!");
            // Kiểm tra xem người đăng nhập có phải là tác giả chính của research topic hay không
            Author_ResearchTopic authorResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>()
            .Entities.FirstOrDefaultAsync(art =>
                art.AuthorId == author.Id &&
                art.ResearchTopicId == researchTopicId &&
                art.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR &&
                art.DeletedAt == null) ??
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN,
            "Bạn không phải là tác giả chính của chủ đề nghiên cứu này!");
            // Tìm Author_ResearchTopic của thành viên cần xóa
            Author_ResearchTopic memberToDelete = await _unitOfWork.GetRepository<Author_ResearchTopic>()
            .Entities.FirstOrDefaultAsync(art =>
                art.AuthorId == memberId &&
                art.ResearchTopicId == researchTopicId &&
                art.RoleName == CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR &&
                art.DeletedAt == null) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND,
            "Thành viên không tồn tại hoặc không phải là đồng tác giả!");
            // Kiểm tra xem research topic có được chấp nhận hay không
            // if (researchTopic.IsAcceptanceApproved == true || researchTopic.IsReviewAcceptance == true)
            // {
            //     throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA,
            //         "Cannot delete member from an approved research topic!");
            // }
            memberToDelete.DeletedAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}