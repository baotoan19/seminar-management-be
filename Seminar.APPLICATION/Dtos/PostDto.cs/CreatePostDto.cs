using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Seminar.APPLICATION.Dtos.PostDto;

public class CreatePostDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; }
    public DateTime DateUpload { get; set; }
    public IFormFile? FilePath { get; set; }
}