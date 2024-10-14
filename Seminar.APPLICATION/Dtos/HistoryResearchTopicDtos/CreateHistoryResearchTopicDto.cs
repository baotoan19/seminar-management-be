using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;

public class CreateHistoryResearchTopicDto
{
    [Required(ErrorMessage = "ResearchTopicId is required")]
    public int ResearchTopicId { get; set; }
    [Required(ErrorMessage = "NewFilePath is required")]
    public string NewFilePath { get; set; }
    [Required(ErrorMessage = "DateUpdate is required")]
    public DateTime DateUpdate { get; set; }
    public string? Summary { get; set; }
}

