namespace Seminar.APPLICATION.Models;

public class ReviewCommitteeVM
{
    public int Id { get; set; }
    public string ReviewCommitteeName { get; set; }
    public int ConferenceId { get; set; }
    public string ConferenceName { get; set; }
}