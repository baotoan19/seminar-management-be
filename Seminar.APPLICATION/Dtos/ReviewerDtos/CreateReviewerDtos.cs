using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewerDtos
{
    public class CreateReviewerDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số.")]
        public string? NumberPhone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Sex { get; set; }
    }
}