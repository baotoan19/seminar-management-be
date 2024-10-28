using System.ComponentModel.DataAnnotations.Schema;
using Seminar.APPLICATION.Dtos.AuthorDtos;

namespace Seminar.APPLICATION.Models;

public class ArticleVM
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string KeyWord { get; set; }
    public string FilePath { get; set; }
    public DateTime DateUpload { get; set; }
    public int DisciplineId { get; set; }
    public string DisciplineName { get; set; }
    public bool IsAcceptedForPublication { get; set; }
    public List<ArticleAuthorVM> CoAuthors { get; set; }

}