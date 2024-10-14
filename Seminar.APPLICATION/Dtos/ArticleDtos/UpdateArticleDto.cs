using System.ComponentModel.DataAnnotations;
namespace Seminar.APPLICATION.Dtos.ArticleDtos;

public class UpdateArticleDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Keyword is required")]
    public List<string> Keywords { get; set; }
    [Required(ErrorMessage = "FilePath is required")]
    public string FilePath { get; set; }
    [Required(ErrorMessage = "DateUpload is required")]
    public DateTime DateUpload { get; set; }
    [Required(ErrorMessage = "DisciplineId is required")]
    public int DisciplineId { get; set; }
    [Required(ErrorMessage = "IsStatus is required")]
    public bool IsStatus { get; set; }
}