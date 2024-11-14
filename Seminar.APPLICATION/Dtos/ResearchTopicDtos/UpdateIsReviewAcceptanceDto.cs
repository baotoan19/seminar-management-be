using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ResearchTopicDtos;

public class UpdateIsReviewAcceptanceDto
{
    public int ResearchTopicId { get; set; }
    [Range(0, 2, ErrorMessage = "Review acceptance status must be 0, 1, or 2")]
    public int ReviewAcceptanceStatus { get; set; }
}