using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;

public class CUReviewAssignmentDto
{
    [Required(ErrorMessage = "OrganizerId is required")]
    public int OrganizerId { get; set; }
    [Required(ErrorMessage = "ReviewerId is required")]
    public int ReviewerId { get; set; }
    [Required(ErrorMessage = "ArticalId is required")]
    public int ArticalId { get; set; }
    [Required(ErrorMessage = "IsAccepted is required")]
    public bool Status { get; set; }
}