using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IReviewCommitteeService
{
    Task<ReviewCommitteeVM> GetReviewCommitteeByIdAsync(int id);
    Task CreateReviewCommitteeAsync(CUReviewCommitteeDto dto);
    Task UpdateReviewCommitteeAsync(int id, CUReviewCommitteeDto dto);
    Task DeleteReviewCommitteeAsync(int id);
}