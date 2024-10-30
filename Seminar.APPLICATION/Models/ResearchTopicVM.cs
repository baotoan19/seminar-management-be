namespace Seminar.APPLICATION.Models;

public class ResearchTopicVM
{
    public int Id { get; set; }
    public string NameTopic { get; set; }
    public DateTime DateUpLoad { get; set; }
    public string Description { get; set; }
    public string Target { get; set; }
    public string AchievedResult { get; set; }
    public bool IsAcceptanceApproved { get; set; }
    public bool IsReviewerAcceptance { get; set; }
    public string ProductFilePath { get; set; }
    public string ReportFilePath { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public int ArticleId { get; set; }
    public string ArticleName { get; set; }
    public int DisciplineId { get; set; }
    public string DisciplineName{ get; set; }
    public int CompetitionId { get; set; }
    public string CompetitionName { get; set; }
    public string Supervisor { get; set; }
    public List<ResearchTopicAuthorVM> CoAuthors { get; set; }
}