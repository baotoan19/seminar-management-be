using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
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
    public string? Supervisor { get; set; } = null;
    [Required(ErrorMessage = "Summary is required")]
    public string Summary { get; set; }
    public string? ProductFilePath { get; set; } = null;
    [Required(ErrorMessage = "ReportFilePath is required")]
    public string ReportFilePath { get; set; }
    public int? ArticleId { get; set; } = null;
    [Required(ErrorMessage = "DisciplineId is required")]
    public int DisciplineId { get; set; }
    [Required(ErrorMessage = "CompetitionId is required")]
    public int CompetitionId { get; set; }
    public List<CoAuthorDto>? CoAuthors { get; set; }
}
