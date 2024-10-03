using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IReviewFormService
{
    public Task<ReviewFormVM> GetReviewFormByIdAsync(int id);
    public Task CreateReviewFormAsync(CUReviewFormDto createReviewFormDto);
    public Task UpdateReviewFormAsync(int id, CUReviewFormDto updateReviewFormDto);
    public Task DeleteReviewFormAsync(int id);
}