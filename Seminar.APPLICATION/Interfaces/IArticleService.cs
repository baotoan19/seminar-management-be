
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IArticleService
{
    Task<ArticleVM> GetArticleByIdAsync(int id);
    Task CreateArticalAsync(CreateArticleDto createArticalsDto);
    Task UpdateArticalAsync(int id, UpdateArticleDto updateArticalsDto);
    Task DeleteArticalAsync(int id);
}