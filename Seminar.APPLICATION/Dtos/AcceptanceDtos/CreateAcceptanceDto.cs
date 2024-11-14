using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.AcceptanceDtos;

public class CreateAcceptanceDto
{
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; }
    [Required(ErrorMessage = "ResearchTopicId is required!")]
    public int ResearchTopicId { get; set; }
}
