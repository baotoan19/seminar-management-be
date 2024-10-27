using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.CompetitionDtos;

public class CreateCompetitionDto
{
    [Required(ErrorMessage = "Competition name is required")]
    public string CompetitionName { get; set; }
    [Required(ErrorMessage = "Date start is required")]
    public DateTime DateStart { get; set; }
    [Required(ErrorMessage = "Date end is required")]
    public DateTime DateEnd { get; set; }
    [Required(ErrorMessage = "Date end submit is required")]
    public DateTime DateEndSubmit {get; set;}
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Destination is required")]
    public string Destination { get; set; }
}