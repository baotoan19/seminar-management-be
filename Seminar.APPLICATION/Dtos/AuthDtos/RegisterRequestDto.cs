using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.AuthDtos
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, including uppercase, lowercase, numbers, and special characters.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        public string NumberPhone { get; set; }
        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; }
    }

}
