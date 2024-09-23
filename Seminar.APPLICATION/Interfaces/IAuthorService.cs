using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Models;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces
{
    public interface IAuthorService
    {
        Task<Author> CreateAuthorAsync(CreateAuthorDto createAuthorDto);
        Task<AuthorVM> GetAuthorInforAsync(int id);
        Task UpdateAuthorAsync(int accountId, UpdateAuthorDto updateAuthorDto);
        Task<AuthorVM> GetCoAuthorByIdAsync(int idCoAuthor);
        Task CreateCoAuthorAsync(CreateAuthorDto createAuthorDto);
        Task UpdateCoAuthorAsync(int idCoAuthor, UpdateAuthorDto updateAuthorDto);
        Task DeleteCoAuthorAsync(int idCoAuthor);
    }
}