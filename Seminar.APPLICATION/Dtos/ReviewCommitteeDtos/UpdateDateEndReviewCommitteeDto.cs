using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;

public class UpdateDateEndReviewCommitteeDto
{
    [Range(1, 12, ErrorMessage = "Month is invalid!")]
    public int Month { get; set; }
}
