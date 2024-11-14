using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ResearchTopicDtos;

public class UpdateDateEndResearchTopicDto
{
    [Range(1, 12, ErrorMessage = "Month is invalid!")]
    public int Month { get; set; }
}
