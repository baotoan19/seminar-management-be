using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.PostDto;

public class UpdatePostDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; }
    public DateTime DateUpload { get; set; }
    public string? FilePath { get; set; }
    public bool IsStatus { get; set; }
}