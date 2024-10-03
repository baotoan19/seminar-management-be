using Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IReviewAssignmentService
{
    Task<ReviewAssignmentVM> GetReviewAssignmentByIdAsync(int id);
    Task CreateReviewAssignmentAsync(CUReviewAssignmentDto dto);
    Task UpdateReviewAssignmentAsync(int id, CUReviewAssignmentDto dto);
    Task DeleteReviewAssignmentAsync(int id);
}