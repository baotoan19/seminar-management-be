using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;

public class CreateHistoryResearchTopicDto
{
    [Required(ErrorMessage = "ResearchTopicId is required")]
    public int ResearchTopicId { get; set; }
    [Required(ErrorMessage = "NewFilePath is required")]
    public string NewReportFilePath { get; set; }
    public string? NewProductFilePath { get; set; }
    [Required(ErrorMessage = "DateUpdate is required")]
    public string? Summary { get; set; }
}

