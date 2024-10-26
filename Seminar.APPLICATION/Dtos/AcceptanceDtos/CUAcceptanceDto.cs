using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.AcceptanceDtos;

public class CUAcceptanceDto
{
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; }
    [Required(ErrorMessage = "ResearchTopicId is required!")]
    public int ResearchTopicId { get; set; }
    [Required(ErrorMessage = "Title is required!")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Content is required!")]
    public string Content { get; set; }
    [Required(ErrorMessage = "IsAcceptedForPublication is required!")]
    public bool IsAcceptedForPublication { get; set; }
}
