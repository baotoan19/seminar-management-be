using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.HistoryArticasDtos;

public class CreateHistoryArticalsDto
{
    [Required(ErrorMessage = "ArticalId is required")]
    public int ArticalId { get; set; }
    [Required(ErrorMessage = "NewFilePath is required")]
    public string NewFilePath { get; set; }
    [Required(ErrorMessage = "DateUpdate is required")]
    public DateTime DateUpdate { get; set; }
    public string? Summary { get; set; }
}