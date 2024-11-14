namespace Seminar.APPLICATION.Models;

public class AuthorArticleVM
{
    public int Id { get; set; }
    public int? AuthorId { get; set; }
    public int? ArticleId { get; set; }
    public string RoleName { get; set; }
    public AuthorVM Author { get; set; }
    public DateTime? DeletedAt { get; set; }
}