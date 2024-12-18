using Newtonsoft.Json;

namespace Seminar.APPLICATION.Models;

public class ResearchTopicVM
{
    public int Id { get; set; }
    public string NameTopic { get; set; }
    public DateTime DateUpLoad { get; set; }
    public string Description { get; set; }
    public string Target { get; set; }
    public float Budget { get; set; }
    public int ProjectDuration { get; set; }
    public string AchievedResults { get; set; }
    public int AcceptanceApprovedStatus { get; set; }
    public int ReviewAcceptanceStatus { get; set; }
    public string BudgetFilePath { get; set; }
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
    public ReviewCommitteeVM Review_Committees { get; set; }
    public string Supervisor { get; set; }
    public ICollection<AuthorResearchTopicVM> Author_ResearchTopics { get; set; }
    public ICollection<HistoryUpdateResearchTopicVM> History_Update_ResearchTopics { get; set; }
    public AcceptanceVM Acceptance { get; set; }
}