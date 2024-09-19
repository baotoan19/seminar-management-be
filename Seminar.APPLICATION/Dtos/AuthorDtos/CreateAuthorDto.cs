using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Seminar.APPLICATION.Dtos.AuthorDtos
{

    public class CreateAuthorDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Phone number is not valid")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits")]
        public string ?NumberPhone { get; set; }
        public string? InternalCode { get; set; }
        public int? AccountId { get; set; }
        public int? FacultyId { get; set; }
    }

}