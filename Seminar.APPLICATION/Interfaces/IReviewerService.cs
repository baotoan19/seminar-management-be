using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.APPLICATION.Models;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces
{
    public interface IReviewerService
    {
        Task<Reviewer> CreateReviewerAsync(CreateReviewerDto createReviewerDto);
        Task<ReviewerVM> GetReviewerInforAsync(int id);
        Task UpdateReviewerAsync(int id, UpdateReviewerDto updateReviewerDto);
    }
}