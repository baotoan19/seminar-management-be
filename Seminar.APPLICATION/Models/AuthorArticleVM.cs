namespace Seminar.APPLICATION.Models;
public class AuthorArticleVM
{
    public required int AuthorId { get; set; }
    public required string AuthorName { get; set; }
    public string? Email { get; set; }
    public string? NumberPhone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Sex { get; set; }
    public int? FacultyId { get; set; }
    public string? FacultyName { get; set; }
    public string? InternalCode { get; set; }
    public int ArticleId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string KeyWord { get; set; }
    public string FilePath { get; set; }
    public DateTime DateUpload { get; set; }
    public int DisciplineId { get; set; }
    public string DisciplineName { get; set; }
    public string RoleName {get; set;}
}