using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewAcceptanceDtos;

public class CreateReviewAcceptanceDto
{
    [Required(ErrorMessage = "AcceptanceId is required!")]
    public int AcceptanceId { get; set; }
    [Required(ErrorMessage = "Description is required!")]
    public string Description { get; set; }
    [Required(ErrorMessage = "IsApproved is required!")]
    public bool IsAccepted { get; set; } 
}
