namespace Seminar.APPLICATION.Models;

public class AuthorResearchTopicVM
{
    public int ResearchTopicId { get; set; }
    public int AuthorId { get; set; }
    public required string RoleName { get; set; }
    public AuthorVM Author { get; set; }
    public DateTime? DeletedAt { get; set; }
}