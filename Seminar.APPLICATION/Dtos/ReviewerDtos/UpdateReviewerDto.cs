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
    [Required(ErrorMessage = "Discipline id is required")]
    public int DisciplineId { get; set; }
    [Required(ErrorMessage = "Review committee id is required")]
    public int ReviewCommitteeId { get; set; }
}
