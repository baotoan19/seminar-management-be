
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces
{
    public interface IUserService
    {
        Task<UserVM> GetUserInforAsync();
        Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto);
        Task UpdateReviewerAsync(UpdateReviewerDto updateReviewerDto);
        Task UpdateOrganizerAsync(UpdateOrganizerDto updateOrganizerDto);
        Task CreateCoAuthorAsync(CreateAuthorDto createAuthorDto);
        Task UpdateCoAuthorAsync(int idCoAuthor, UpdateAuthorDto updateAuthorDto);
        Task DeleteCoAuthorAsync(int idCoAuthor);
        Task<AuthorVM> GetCoAuthorByIdAsync(int idCoAuthor);
    }
}