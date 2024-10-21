namespace Seminar.APPLICATION.Dtos.CompetitionDtos;

public class UpdateCompetitionDto
{
    public string CompetitionName { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string Description { get; set; }
    public string Destination { get; set; }
}