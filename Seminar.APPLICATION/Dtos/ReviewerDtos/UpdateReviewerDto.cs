using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewerDtos;

public class UpdateReviewerDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Phone number is required")]
    public string NumberPhone { get; set; }
    [Required(ErrorMessage = "Academic rank is required")]
    public string AcademicRank { get; set; }
    [Required(ErrorMessage = "Academic degree is required")]
    public string AcademicDegree { get; set; }
    [Required(ErrorMessage = "Faculty id is required")]
    public int FacultyId { get; set; }
    [Required(ErrorMessage = "Account id is required")]
    public int AccountId { get; set; }
}
