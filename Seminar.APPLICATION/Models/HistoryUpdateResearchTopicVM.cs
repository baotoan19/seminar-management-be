namespace Seminar.APPLICATION.Models;

public class HistoryUpdateResearchTopicVM
{
    public int Id {get; set;}
    public int ResearchTopicId {get; set;}
    public string NameTopic {get; set;}
    public string NewReportFilePath {get; set;}
    public string NewProductFilePath {get; set;}
    public DateTime DateUpdate {get; set;}
    public string Summary {get; set;}
}