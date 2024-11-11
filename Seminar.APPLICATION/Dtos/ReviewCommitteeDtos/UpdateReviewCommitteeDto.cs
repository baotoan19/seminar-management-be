using System.ComponentModel.DataAnnotations;
namespace Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;

public class UpdateReviewCommitteeDto
{
    [Required(ErrorMessage = "Review committee name is required")]
    public string ReviewCommitteeName { get; set; }
    [Required(ErrorMessage = "Date start is required")]
    public DateTime DateStart { get; set; }
    [Required(ErrorMessage = "Date end is required")]
    public DateTime DateEnd { get; set; }
    public ICollection<ReviewBoardMemberDto>? ReviewBoardMembers { get; set; }
}



