using System.ComponentModel.DataAnnotations;
using Seminar.APPLICATION.Dtos.AuthorDtos;
namespace Seminar.APPLICATION.Dtos.ResearchTopicDtos;

public class CreateResearchTopicDto
{
    [Required(ErrorMessage = "NameTopic is required")]
    public string NameTopic { get; set; }
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Target is required")]
    public string Target { get; set; }
    [Required(ErrorMessage = "AchievedResults is required")]
    public string AchievedResults { get; set; }
    [Required(ErrorMessage = "Budget is required")]
    public float Budget { get; set; }
    [Required(ErrorMessage = "ProjectDuration is required")]
    public int ProjectDuration { get; set; }
    public string? Supervisor { get; set; } = string.Empty;
    [Required(ErrorMessage = "Summary is required")]
    public string Summary { get; set; }
    public string? ProductFilePath { get; set; } = string.Empty;
    public string? BudgetFilePath { get; set; } = string.Empty;
    [Required(ErrorMessage = "ReportFilePath is required")]
    public string ReportFilePath { get; set; }
    public int? ArticleId { get; set; } = 0;
    [Required(ErrorMessage = "DisciplineId is required")]
    public int DisciplineId { get; set; }
    [Required(ErrorMessage = "CompetitionId is required")]
    public int CompetitionId { get; set; }
    public List<CoAuthorDto>? CoAuthors { get; set; }
}
