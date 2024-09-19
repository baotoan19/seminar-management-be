using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.AuthDtos;
public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
    ErrorMessage = "Password must be at least 8 characters long, including uppercase, lowercase, numbers, and special characters.")]
    public string Password { get; set; }
}