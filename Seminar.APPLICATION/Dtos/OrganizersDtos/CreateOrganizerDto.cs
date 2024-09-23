using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.OrganizersDtos
{
    public class CreateOrganizerDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits")]
        public string? NumberPhone { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "AccountId is required")]
        public int AccountId { get; set; }
        public int? FacultyId { get; set; }
    }
}