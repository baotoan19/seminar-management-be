using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewerDtos
{
    public class CreateReviewerDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số.")]
        public string? NumberPhone { get; set; }
        public string? AcademicDegree { get; set; }
        public string? AcademicRank { get; set; }
        public int? DisciplineId { get; set; }
        public int? ReviewCommitteeId { get; set; }
        [Required(ErrorMessage = "AccountId is required")]
        public int AccountId { get; set; }
    }
}