using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.EmailDtos;

public class EmailFeedBackDto
{
    public string EmailReviewer { get; set; }
    public string NameReviewer { get; set; }
    public string NameAuthor { get; set; }
    public string TitleResearchTopic { get; set; }
    public DateTime ProposedTime { get; set; }
    public string Feedback { get; set; }
    public string EmailAuthor { get; set; }
    public string PhoneAuthor { get; set; }
}
