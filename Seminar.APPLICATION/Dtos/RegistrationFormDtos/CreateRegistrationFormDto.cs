using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Seminar.APPLICATION.Dtos.RegistrationFormDtos;

public class CreateRegistrationFormDto
{
    [Required(ErrorMessage = "Competition Id is required!")]
    public int CompetitionId { get; set; }
    [Required(ErrorMessage = "File Path is required!")]
    public string FilePath { get; set; }
}