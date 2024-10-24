using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.RegistrationFormDtos;

public class UpdateRegistrationFormDto
{
    [Required(ErrorMessage = "Is Accepted is required!")]
    public int IsAccepted { get; set; }
}