using System.Data.Common;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Services;
public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IFirebaseService _firebaseService;

    public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _firebaseService = firebaseService;
    }

    public async Task<PaginatedList<ArticleVM>> GetAllArticlesPagedAsync(int index = 1, int pageSize = 8, string idSearch = "", string nameSearch = "")
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }

        IQueryable<Article> query = _unitOfWork.GetRepository<Article>().Entities
            .Include(a => a.Discipline)
                .Where(a => a.DeletedAt == null)
                    .OrderByDescending(a => a.CreatedAt);
        //Tìm kiếm theo id
        if (!string.IsNullOrEmpty(idSearch))
        {
            query = query.Where(p => p.Id.ToString().Contains(idSearch));
            bool isInt = int.TryParse(idSearch, out int idInt);
            if (!isInt)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
            }
        }
        //Tìm kiếm theo tên
        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
            }
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ArticleVM>(new List<ArticleVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
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

    public async Task<PaginatedList<ArticleVM>> GetApprovedArticlesPagedAsync(int index = 1, int pageSize = 8, string idSearch = "", string nameSearch = "")
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }

        IQueryable<Article> query = _unitOfWork.GetRepository<Article>().Entities
            .Include(a => a.Discipline)
                .Where(a => a.DeletedAt == null && a.IsAcceptedForPublication == true)
                    .OrderByDescending(a => a.CreatedAt);
        //Tìm kiếm theo id
        if (!string.IsNullOrEmpty(idSearch))
        {
            query = query.Where(p => p.Id.ToString().Contains(idSearch));
            bool isInt = int.TryParse(idSearch, out int idInt);
            if (!isInt)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
            }
        }
        //Tìm kiếm theo tên
        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
            }
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ArticleVM>(new List<ArticleVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
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
        Article article = await _unitOfWork.GetRepository<Article>().Entities.Include(a => a.Discipline)
        .FirstOrDefaultAsync(a => a.Id == id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        ArticleVM articleVM = _mapper.Map<ArticleVM>(article);
        return articleVM;
    }

    public async Task CreateArticleAsync(CreateArticleDto createArticalsDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Article article = _mapper.Map<Article>(createArticalsDto);
        article.Discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(createArticalsDto.DisciplineId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        string keyword = string.Join(",", createArticalsDto.Keywords);
        article.KeyWord = keyword;
        article.IsAcceptedForPublication = false;
        await _unitOfWork.GetRepository<Article>().InsertAsync(article);
        await _unitOfWork.SaveChangesAsync();
        //Insert author_article
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        Author_Article author_Article = new Author_Article
        {
            AuthorId = author.Id,
            ArticleId = article.Id,
            RoleName = "author"
        };
        await _unitOfWork.GetRepository<Author_Article>().InsertAsync(author_Article);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto)
    {
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
        article.Discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(updateArticleDto.DisciplineId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        _mapper.Map(updateArticleDto, article);
        string keyword = string.Join(",", updateArticleDto.Keywords);
        article.KeyWord = keyword;
        article.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteArticleAsync(int id)
    {
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
        article.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<AuthorArticleVM>> GetAuthorArticleByRoleAsync(string roleName)
    {
        if (roleName.ToLower() != CLAIMS_VALUES.ROLE_TYPE.AUTHOR && roleName.ToLower() != CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid role name");
        }
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities
            .FirstOrDefaultAsync(a => a.AccountId == userId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        List<Author_Article> authorArticles = await _unitOfWork.GetRepository<Author_Article>().Entities
            .Include(aa => aa.Author)
            .ThenInclude(a => a.Faculty)
            .Where(aa => aa.AuthorId == author.Id && aa.RoleName == roleName)
            .Include(aa => aa.Article)
            .ThenInclude(a => a.Discipline)
            .Where(aa => aa.Article != null && aa.Article.IsAcceptedForPublication == true && aa.Article.DeletedAt == null)
            .ToListAsync();

        List<AuthorArticleVM> authorArticleVMs = authorArticles
            .Select(aa => new AuthorArticleVM
            {
                // Authors
                AuthorId = aa.Author.Id,
                AuthorName = aa.Author.Name ?? "Unknown",
                Email = aa.Author.Email ?? "No email",
                NumberPhone = aa.Author.NumberPhone ?? "No phone",
                FacultyId = aa.Author.FacultyId,
                FacultyName = aa.Author.Faculty?.FacultyName ?? "Unknown Faculty",
                InternalCode = aa.Author.InternalCode ?? "No code",
                // Articles
                ArticleId = aa.Article.Id,
                Title = aa.Article.Title ?? "Untitled",
                Description = aa.Article.Description ?? "No description",
                KeyWord = aa.Article.KeyWord ?? "No keywords",
                FilePath = aa.Article.FilePath ?? "No file",
                DateUpload = aa.Article.CreatedAt,
                DisciplineId = aa.Article.DisciplineId ?? 0,
                DisciplineName = aa.Article.Discipline.DisciplineName ?? "Unknown Discipline",
                RoleName = aa.RoleName ?? "Unknown Role"
            }).ToList();

        return authorArticleVMs;
    }
    public async Task<List<AuthorArticleVM>> GetAuthorArticleByAuthorIdAsync()
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities
            .FirstOrDefaultAsync(a => a.AccountId == userId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        List<Author_Article> authorArticles = await _unitOfWork.GetRepository<Author_Article>().Entities
            .Include(aa => aa.Author)
            .ThenInclude(a => a.Faculty)
            .Where(aa => aa.AuthorId == author.Id)
            .Include(aa => aa.Article)
            .ThenInclude(a => a.Discipline)
            .Where(aa => aa.Article != null && aa.Article.IsAcceptedForPublication == true && aa.Article.DeletedAt == null && aa.DeletedAt == null)
            .ToListAsync();
        List<AuthorArticleVM> authorArticleVMs = authorArticles
            .Select(aa => new AuthorArticleVM
            {
                // Authors
                AuthorId = aa.Author.Id,
                AuthorName = aa.Author.Name ?? "Unknown",
                Email = aa.Author.Email ?? "No email",
                NumberPhone = aa.Author.NumberPhone ?? "No phone",
                DateOfBirth = aa.Author.DateOfBirth,
                Sex = aa.Author.Sex,
                FacultyId = aa.Author.FacultyId,
                FacultyName = aa.Author.Faculty?.FacultyName ?? "Unknown Faculty",
                InternalCode = aa.Author.InternalCode ?? "No code",
                // Articles
                ArticleId = aa.Article.Id,
                Title = aa.Article.Title ?? "Untitled",
                Description = aa.Article.Description ?? "No description",
                KeyWord = aa.Article.KeyWord ?? "No keywords",
                FilePath = aa.Article.FilePath ?? "No file",
                DateUpload = aa.Article.CreatedAt,
                DisciplineId = aa.Article.DisciplineId ?? 0,
                DisciplineName = aa.Article.Discipline.DisciplineName ?? "Unknown Discipline",
                RoleName = aa.RoleName ?? "Unknown Role"
            }).ToList();
        return authorArticleVMs;
    }
    public async Task<List<ArticleAuthorVM>> GetAuthorByArticleIdAsync(int articleId)
    {
        List<Author_Article> authorArticles = await _unitOfWork.GetRepository<Author_Article>().Entities
            .Include(aa => aa.Author)
            .ThenInclude(a => a.Faculty)
            .Where(aa => aa.ArticleId == articleId && aa.DeletedAt == null)
            .ToListAsync() ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
        List<ArticleAuthorVM> authorArticleVMs = authorArticles
            .Select(aa => new ArticleAuthorVM
            {
                Id = aa.Author.Id,
                Name = aa.Author.Name ?? "Unknown",
                Email = aa.Author.Email ?? "No email",
                NumberPhone = aa.Author.NumberPhone ?? "No phone",
                DateOfBirth = aa.Author.DateOfBirth ?? DateTime.MinValue,
                Sex = aa.Author.Sex ?? "Unknown",
                FacultyId = aa.Author.FacultyId,
                FacultyName = aa.Author.Faculty?.FacultyName ?? "Unknown Faculty",
                InternalCode = aa.Author.InternalCode ?? "No code",
                RoleName = aa.RoleName ?? "Unknown Role"
            }).ToList();
        return authorArticleVMs;
    }

    public async Task ApproveArticleAsync(int id, ApproveArticleDto approveArticleDto)
    {
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Article not found!");
        article.IsAcceptedForPublication = approveArticleDto.IsAcceptedForPublication;
        article.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }
}