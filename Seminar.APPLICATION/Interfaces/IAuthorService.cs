using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces
{
    public interface IAuthorService
    {
        Task<Author> CreateAuthorAsync(CreateAuthorDto createAuthorDto);
    }
}