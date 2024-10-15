using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewerDtos
{
    public class CreateReviewerDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số.")]
        public string? NumberPhone { get; set; }
        public string? AcademicDegree { get; set; }
        public string? AcademicRank { get; set; }
        public int? FacutyId { get; set; }
        public int? Review_CommitteeId { get; set; }
        [Required(ErrorMessage = "AccountId is required")]
        public int AccountId { get; set; }
    }
}