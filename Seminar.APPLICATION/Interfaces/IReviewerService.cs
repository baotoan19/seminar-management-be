using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces
{
    public interface IReviewerService
    {
        Task<Reviewer> CreateReviewerAsync(CreateReviewerDto createReviewerDto);
    }
}