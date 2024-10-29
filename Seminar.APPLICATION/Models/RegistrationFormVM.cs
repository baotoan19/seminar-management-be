namespace Seminar.APPLICATION.Models;

public class RegistrationFormVM
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int AccountId { get; set; }
    public string? AuthorName { get; set; }
    public string? InternalCode { get; set; }
    public int CompetitionId { get; set; }
    public string? CompetitionName { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string FilePath { get; set; }
    public int IsAccepted { get; set; }
}