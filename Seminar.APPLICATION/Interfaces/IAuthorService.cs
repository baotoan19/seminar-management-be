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
    }
}