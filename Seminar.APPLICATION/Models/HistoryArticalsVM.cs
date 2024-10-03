namespace Seminar.APPLICATION.Models;

public class HistoryArticalsVM
{
    public int Id { get; set; }
    public int ArticalId { get; set; }
    public string Title { get; set; }
    public string NewFilePath { get; set; }
    public DateTime DateUpdate { get; set; }
    public string? Summary { get; set; }
}