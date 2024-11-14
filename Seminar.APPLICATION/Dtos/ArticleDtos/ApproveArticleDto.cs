using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ArticleDtos;

public class ApproveArticleDto
{
    [Range(0, 2, ErrorMessage = "Accepted for publication status must be between 0 and 2")]
    public int AcceptedForPublicationStatus { get; set; }
}
