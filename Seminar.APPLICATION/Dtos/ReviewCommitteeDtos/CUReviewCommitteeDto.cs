using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;

public class CUReviewCommitteeDto
{
    [Required(ErrorMessage = "Review Committee Name is required!")]
    public string ReviewCommitteeName { get; set; }
    [Required(ErrorMessage = "Conference Id is required!")]
    public int ConferenceId { get; set; }
}