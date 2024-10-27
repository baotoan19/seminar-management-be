using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.AuthorDtos;

public class CoAuthorDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Phone number is required")]
    public string NumberPhone { get; set; }
    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }
    [Required(ErrorMessage = "Sex is required")]
    public string Sex { get; set; }
}
