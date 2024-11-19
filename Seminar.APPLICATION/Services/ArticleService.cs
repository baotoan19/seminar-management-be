using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.ArticleDtos;
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
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Services;
public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IAuthorService _authorService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ArticleService> _logger;

    public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmailService emailService, ILogger<ArticleService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<PaginatedList<ArticleVM>> GetAllArticlesPagedAsync(int index, int pageSize, string idSearch, string nameSearch, int acceptedForPublicationStatus)
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ!");
        }

        IQueryable<Article> query = _unitOfWork.GetRepository<Article>().Entities
                .Where(a => a.DeletedAt == null)
                    .OrderByDescending(a => a.CreatedAt);
        //Tìm kiếm theo id
        if (!string.IsNullOrEmpty(idSearch))
        {
            query = query.Where(p => p.Id.ToString().Contains(idSearch));
            bool isInt = int.TryParse(idSearch, out int idInt);
            if (!isInt)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
            }
        }
        //Tìm kiếm theo tên
        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
            }
        }

        switch (acceptedForPublicationStatus)
        {
            case (int)AcceptedForPublicationStatusEnum.Pending:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Pending);
                break;
            case (int)AcceptedForPublicationStatusEnum.Approved:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved);
                break;
            case (int)AcceptedForPublicationStatusEnum.Rejected:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Rejected);
                break;
            case (int)AcceptedForPublicationStatusEnum.All:
                break;
            default:
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Trạng thái duyệt để xuất bản không hợp lệ!");
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ArticleVM>(new List<ArticleVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        foreach (var article in resultQuery)
        {
            article.Author_Articles = article.Author_Articles
                .Where(aa => aa.DeletedAt == null)
                .ToList();
        }
        List<ArticleVM> responeItems = _mapper.Map<List<ArticleVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ArticleVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<ArticleVM> GetArticleByIdAsync(int id)
    {
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
        ArticleVM articleVM = _mapper.Map<ArticleVM>(article);
        articleVM.Author_Articles = articleVM.Author_Articles
            .Where(aa => aa.DeletedAt == null)
            .ToList();
        return articleVM;
    }
    public async Task<PaginatedList<ArticleVM>> GetApprovedArticlesPagedAsync(int index, int pageSize, string idSearch, string nameSearch)
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ!");
        }

        IQueryable<Article> query = _unitOfWork.GetRepository<Article>().Entities
                .Where(a => a.DeletedAt == null && a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved)
                    .OrderByDescending(a => a.CreatedAt);
        //Tìm kiếm theo id
        if (!string.IsNullOrEmpty(idSearch))
        {
            query = query.Where(p => p.Id.ToString().Contains(idSearch));
            bool isInt = int.TryParse(idSearch, out int idInt);
            if (!isInt)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
            }
        }
        //Tìm kiếm theo tên
        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
            }
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ArticleVM>(new List<ArticleVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        foreach (var article in resultQuery)
        {
            article.Author_Articles = article.Author_Articles
                .Where(aa => aa.DeletedAt == null)
                .ToList();
        }
        List<ArticleVM> responeItems = _mapper.Map<List<ArticleVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ArticleVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<PaginatedList<ArticleVM>> GetAllArticlesByAuthorIdPagedAsync(int index, int pageSize, string idSearch, string nameSearch, int acceptedForPublicationStatus, string roleName)
    {
        int userID = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userID) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ!");
        }

        IQueryable<Article> query = _unitOfWork.GetRepository<Article>().Entities
                .Where(a => a.DeletedAt == null && a.Author_Articles.Any(aa => aa.AuthorId == author.Id))
                    .OrderByDescending(a => a.CreatedAt);
        //Tìm kiếm theo id
        if (!string.IsNullOrEmpty(idSearch))
        {
            query = query.Where(p => p.Id.ToString().Contains(idSearch));
            bool isInt = int.TryParse(idSearch, out int idInt);
            if (!isInt)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
            }
        }
        //Tìm kiếm theo tên
        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
            }
        }

        //Lọc theo trạng thái
        switch (acceptedForPublicationStatus)
        {
            case (int)AcceptedForPublicationStatusEnum.Pending:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Pending);
                break;
            case (int)AcceptedForPublicationStatusEnum.Approved:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved);
                break;
            case (int)AcceptedForPublicationStatusEnum.Rejected:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Rejected);
                break;
            case (int)AcceptedForPublicationStatusEnum.All:
                break;
            default:
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Trạng thái duyệt để xuất bản không hợp lệ!");
        }

        //Lọc theo role
        if (string.IsNullOrEmpty(roleName))
        {
            query = query.Where(a => a.Author_Articles.Any(aa => aa.AuthorId == author.Id));
        }
        else
        {
            switch (roleName)
            {
                case CLAIMS_VALUES.ROLE_TYPE.AUTHOR:
                    query = query.Where(a => a.Author_Articles.Any(aa => aa.AuthorId == author.Id && aa.RoleName == roleName));
                    break;
                case CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR:
                    query = query.Where(a => a.Author_Articles.Any(aa => aa.AuthorId == author.Id && aa.RoleName == roleName));
                    break;
                default:
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Tên vai trò không hợp lệ!");
            }
        }


        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ArticleVM>(new List<ArticleVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        foreach (var article in resultQuery)
        {
            article.Author_Articles = article.Author_Articles
                .Where(aa => aa.DeletedAt == null)
                .ToList();
        }
        List<ArticleVM> responeItems = _mapper.Map<List<ArticleVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ArticleVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task CreateArticleAsync(CreateArticleDto createArticalsDto)
    {
        Discipline discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(createArticalsDto.DisciplineId) ?? throw
        new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        var strategy = _unitOfWork.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
                    Article article = _mapper.Map<Article>(createArticalsDto);
                    article.Discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(createArticalsDto.DisciplineId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lĩnh vực không tồn tại!");
                    string keyword = string.Join(",", createArticalsDto.Keywords);
                    article.KeyWord = keyword;
                    article.AcceptedForPublicationStatus = (int)AcceptedForPublicationStatusEnum.Pending;
                    await _unitOfWork.GetRepository<Article>().InsertAsync(article);
                    await _unitOfWork.SaveChangesAsync();
                    //Insert main author
                    Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
                    List<Author_Article> author_Articles = new List<Author_Article>
                    {
                        new Author_Article
                        {
                            AuthorId = author.Id,
                            ArticleId = article.Id,
                            RoleName = CLAIMS_VALUES.ROLE_TYPE.AUTHOR
                        }
                    };
                    //Insert co-authors
                    if (createArticalsDto.CoAuthors != null)
                    {
                        await ValidateCoAuthorEmailsAsync(createArticalsDto.CoAuthors);

                        foreach (CoAuthorDto coAuthorDto in createArticalsDto.CoAuthors)
                        {
                            int coAuthorId = await CreateOrUpdateAuthorAsync(coAuthorDto, article);
                            author_Articles.Add(new Author_Article
                            {
                                AuthorId = coAuthorId,
                                ArticleId = article.Id,
                                RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR
                            });
                        }

                    }
                    await _unitOfWork.GetRepository<Author_Article>().InsertRangeAsync(author_Articles);
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
    private async Task ValidateCoAuthorEmailsAsync(List<CoAuthorDto> coAuthors)
    {
        var processedEmails = new HashSet<string>();
        foreach (CoAuthorDto coAuthor in coAuthors)
        {
            // Kiểm tra email trùng lặp trong danh sách
            if (!processedEmails.Add(coAuthor.Email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Email của đồng tác giả bị trùng lặp!");
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
    private async Task<int> CreateOrUpdateAuthorAsync(CoAuthorDto coAuthorDto, Article article)
    {
        Author? existingAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);

        if (existingAuthor == null)
        {
            // Tạo tài khoản và author mới nếu không tồn tại
            Role role = await _unitOfWork.GetRepository<Role>().Entities
            .FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Vai trò không tồn tại!");

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
            await _emailService.SendCoAuthorAccountInfoEmail(coAuthorDto);
            return newAuthor.Id;
        }
        else
        {
            Author_Article? existingAuthorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
                .FirstOrDefaultAsync(a => a.AuthorId == existingAuthor.Id && a.ArticleId == article.Id);
            if (existingAuthorArticle != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Đồng tác giả đã tồn tại!");
            }
            return existingAuthor.Id;
        }
    }
    public async Task UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto)
    {
        // Kiểm tra tài khoản và quyền sở hữu bài báo
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities
            .FirstOrDefaultAsync(a => a.AccountId == userId) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại!");
        // Kiểm tra bài báo có tồn tại không
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
        // Kiểm tra tài khoản và quyền sở hữu bài báo
        Author_Article author_Article = await _unitOfWork.GetRepository<Author_Article>().Entities
            .FirstOrDefaultAsync(a => a.ArticleId == id && a.AuthorId == author.Id && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả bài báo không tồn tại hoặc không phải tác giả!");
        // Kiểm tra bài báo có được phép cập nhật không
        if (article.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved || article.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Rejected)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN,
                "Bạn không thể cập nhật bài báo đã được duyệt hoặc bị từ chối!");
        }
        // Thực hiện cập nhật bài báo
        var strategy = _unitOfWork.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Cập nhật thông tin cơ bản của bài báo
                    _mapper.Map(updateArticleDto, article);
                    string keyword = string.Join(",", updateArticleDto.Keywords);
                    article.KeyWord = keyword;
                    article.UpdatedAt = DateTime.Now;
                    article.DisciplineId = updateArticleDto.DisciplineId;

                    // Cập nhật co-authors nếu có
                    if (updateArticleDto.CoAuthors != null && updateArticleDto.CoAuthors.Count > 0)
                    {
                        // Get existing co-authors (including deleted ones)
                        List<Author_Article> existingCoAuthors = await _unitOfWork.GetRepository<Author_Article>().Entities
                            .Where(a => a.ArticleId == id && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR)
                            .ToListAsync();

                        // Lấy danh sách email từ yêu cầu
                        var newCoAuthorEmails = updateArticleDto.CoAuthors.Select(x => x.Email.ToLower()).ToList();

                        // Đánh dấu co-authors cần xóa nếu không có trong yêu cầu
                        var coAuthorsToDelete = existingCoAuthors.Where(x => x.DeletedAt == null &&
                            !newCoAuthorEmails.Contains((x.Author?.Email ?? string.Empty).ToLower())).ToList();
                        foreach (var coAuthor in coAuthorsToDelete)
                        {
                            coAuthor.DeletedAt = DateTime.Now;
                        }

                        // Xử lý co-authors mới
                        List<Author_Article> author_Articles = new List<Author_Article>();
                        foreach (var coAuthorDto in updateArticleDto.CoAuthors)
                        {
                            await ValidateCoAuthorEmailsAsync(updateArticleDto.CoAuthors);

                            // Bỏ qua nếu co-author đã tồn tại và chưa bị xóa
                            var activeCoAuthor = existingCoAuthors.FirstOrDefault(x =>
                                x.Author.Email?.ToLower() == coAuthorDto.Email.ToLower() && x.DeletedAt == null);
                            if (activeCoAuthor != null)
                            {
                                continue;
                            }

                            // Kích hoạt lại co-author nếu tồn tại
                            var deletedCoAuthor = existingCoAuthors.FirstOrDefault(x =>
                                x.Author.Email?.ToLower() == coAuthorDto.Email.ToLower() && x.DeletedAt != null);
                            if (deletedCoAuthor != null)
                            {
                                deletedCoAuthor.DeletedAt = null;
                                Author? authorToUpdate = await _unitOfWork.GetRepository<Author>()
                                    .Entities.FirstOrDefaultAsync(a => a.Id == deletedCoAuthor.AuthorId);
                                if (authorToUpdate != null)
                                {
                                    authorToUpdate.Name = coAuthorDto.Name;
                                    authorToUpdate.NumberPhone = coAuthorDto.NumberPhone;
                                    authorToUpdate.DateOfBirth = coAuthorDto.DateOfBirth;
                                    authorToUpdate.Sex = coAuthorDto.Sex;
                                    authorToUpdate.UpdatedAt = DateTime.Now;
                                }
                                continue;
                            }

                            // Tạo co-author mới nếu không tồn tại
                            int coAuthorId = await CreateOrUpdateAuthorAsync(coAuthorDto, article);
                            author_Articles.Add(new Author_Article
                            {
                                AuthorId = coAuthorId,
                                ArticleId = article.Id,
                                RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR
                            });
                        }

                        if (author_Articles.Any())
                        {
                            await _unitOfWork.GetRepository<Author_Article>().InsertRangeAsync(author_Articles);
                        }
                    }

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
    public async Task DeleteArticleAsync(int id)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        string roleName = Authentication.GetUserRoleFromHttpContext(_httpContextAccessor.HttpContext);
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại!");
        if (article.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved && roleName.ToLower() != CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN,
                "Chỉ có quản trị viên mới có thể xóa các bài báo đã được xuất bản");
        }
        if (roleName.ToLower() == CLAIMS_VALUES.ROLE_TYPE.AUTHOR)
        {
            Author_Article? authorArticle = await _unitOfWork.GetRepository<Author_Article>().Entities
            .FirstOrDefaultAsync(aa => aa.ArticleId == id && aa.Author.AccountId == userId && aa.RoleName == "author");
            if (authorArticle == null)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN,
                "Bạn chỉ có thể xóa bài báo của chính mình!");
            }
        }
        article.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ApproveArticleAsync(int id, ApproveArticleDto dto)
    {
        // Validate enum value
        if (!Enum.IsDefined(typeof(AcceptedForPublicationStatusEnum), dto.AcceptedForPublicationStatus))
        {
            throw new BadRequestException(
                ResponseCodeConstants.INVALID_STATUS,
                "Trạng thái duyệt để xuất bản không hợp lệ (0: Chờ duyệt, 1: Đã duyệt, 2: Bị từ chối)"
            );
        }

        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id)
            ?? throw new ErrorException(
                StatusCodes.Status404NotFound,
                ResponseCodeConstants.NOT_FOUND,
                "Bài báo không tồn tại!"
            );

        article.AcceptedForPublicationStatus = dto.AcceptedForPublicationStatus;
        await _unitOfWork.SaveChangesAsync();
    }

}

