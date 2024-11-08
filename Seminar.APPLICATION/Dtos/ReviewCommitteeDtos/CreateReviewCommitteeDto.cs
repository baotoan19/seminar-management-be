using System.ComponentModel.DataAnnotations;
using Seminar.APPLICATION.Dtos.ReviewerDtos;

namespace Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;

public class CreateReviewCommitteeDto
{
    [Required(ErrorMessage = "Review committee name is required")]
    public string ReviewCommitteeName { get; set; }
    [Required(ErrorMessage = "Competition is required")]
    public int CompetitionId { get; set; }
    public ICollection<ReviewBoardMemberDto>? ReviewBoardMembers { get; set; }
}

public class ReviewBoardMemberDto : CreateReviewerDto
{
    public string? Description { get; set; }
}



