using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ProceedingsDtos;

public class CUProceedingDto
{
    [Required(ErrorMessage = "Title is required!")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Content is required!")]
    public string Content { get; set; }
    [Required(ErrorMessage = "Image path is required!")]
    public string ImagePath { get; set; }
}