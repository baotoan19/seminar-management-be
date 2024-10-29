
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Models;
using Seminar.DOMAIN.Entitys;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface IArticleService
{
    Task<PaginatedList<ArticleVM>> GetAllArticlesPagedAsync(int index, int pageSize, string idSearch, string nameSearch);
    Task<PaginatedList<ArticleVM>> GetApprovedArticlesPagedAsync(int index, int pageSize, string idSearch, string nameSearch);
    Task<ArticleVM> GetArticleByIdAsync(int id);
    Task CreateArticleAsync(CreateArticleDto createArticleDto);
    Task UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto);
    Task DeleteArticleAsync(int id);
    Task<List<AuthorArticleVM>> GetAuthorArticleByRoleAsync(string roleName);
    Task<List<AuthorArticleVM>> GetAuthorArticleByAuthorIdAsync(bool isAcceptedForPublication);
    Task<List<ArticleAuthorVM>> GetAuthorByArticleIdAsync(int articleId);
    Task ApproveArticleAsync(int id, ApproveArticleDto approveArticleDto);

}