using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;

namespace Seminar.APPLICATION.Dtos.ArticalsDtos;

public class CUArticalsDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Text is required")]
    public string Text { get; set; }
    [Required(ErrorMessage = "Keyword is required")]
    public List<string> Keywords { get; set; }
    [Required(ErrorMessage = "FilePath is required")]
    public string FilePath { get; set; }
    [Required(ErrorMessage = "DateUpload is required")]
    public DateTime DateUpload { get; set; }
    [Required(ErrorMessage = "DisciplineId is required")]
    public int DisciplineId { get; set; }
    [Required(ErrorMessage = "ProceedingId is required")]
    public int ProceedingId { get; set; }
    [Required(ErrorMessage = "ConferenceId is required")]
    public int ConferenceId { get; set; }
}