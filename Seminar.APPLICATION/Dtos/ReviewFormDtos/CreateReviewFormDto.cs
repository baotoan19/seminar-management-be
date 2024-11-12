using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewFormDtos;

public class CreateReviewFormDto
{
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; }
    [Required(ErrorMessage = "History_Update_ResearchTopicId is required")]
    public int History_Update_ResearchTopicId {get; set;}
    [Required(ErrorMessage = "ConcludeId is required")]
    public int ConcludeId {get; set;}
}
