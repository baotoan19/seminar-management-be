namespace Seminar.APPLICATION.Models;

public class RegistrationFormVM
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public int CompetitionId { get; set; }
    public string? CompetitionName { get; set; }
    public string FilePath { get; set; }
    public int IsAccepted { get; set; }
}