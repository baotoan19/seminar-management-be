using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface IReviewFormService
{
    public Task<PaginatedList<ReviewFormVM>> GetAllReviewFormByHistoryUpdateResearchTopicIdAsync(int historyUpdateResearchTopicId, int index, int pageSize);
    public Task CreateReviewFormAsync(CreateReviewFormDto createReviewFormDto);
    public Task UpdateReviewFormAsync(int id, UpdateReviewFormDto updateReviewFormDto);
    public Task DeleteReviewFormAsync(int id);
}