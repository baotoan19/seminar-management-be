using Seminar.APPLICATION.Dtos.ReviewFormDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IReviewFormService
{
    public Task CreateReviewFormAsync(CreateReviewFormDto createReviewFormDto);
    public Task UpdateReviewFormAsync(int id, UpdateReviewFormDto updateReviewFormDto);
    public Task DeleteReviewFormAsync(int id);
}