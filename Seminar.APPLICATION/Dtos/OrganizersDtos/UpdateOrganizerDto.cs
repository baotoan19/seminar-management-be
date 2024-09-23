using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.OrganizersDtos;

public class UpdateOrganizerDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Phone number is required")]
    public string NumberPhone { get; set; }
    [Required(ErrorMessage = "Faculty id is required")]
    public int FacultyId { get; set; }
    public string? Description { get; set; }

}
