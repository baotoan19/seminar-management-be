namespace Seminar.APPLICATION.Models;

public class ReviewFormVM
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int History_Update_ResearchTopicId { get; set; }
    public ReviewerVM Reviewer { get; set; }
    public ConcludeVM Conclude { get; set; }
    public DateTime Date_Upload { get; set; }
}