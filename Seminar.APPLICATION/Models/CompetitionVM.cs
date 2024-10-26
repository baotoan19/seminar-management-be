namespace Seminar.APPLICATION.Models;

public class CompetitionVM
{
    public int Id { get; set; }
    public string CompetitionName { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string Description { get; set; }
    public string Destination { get; set; }
    public int OrganizerId { get; set; }
    public string OrganizerName { get; set; }
}