using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.RegistrationFormDtos;

public class CURegistrationFormDto
{
    [Required(ErrorMessage = "Author Id is required!")]
    public int AuthorId { get; set; }
    [Required(ErrorMessage = "Competition Id is required!")]
    public int CompetitionId { get; set; }
    [Required(ErrorMessage = "File Path is required!")]
    public string FilePath { get; set; }
    [Required(ErrorMessage = "Is Accepted is required!")]
    public int IsAccepted { get; set; }
}