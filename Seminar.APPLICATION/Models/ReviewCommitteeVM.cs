namespace Seminar.APPLICATION.Models;

public class ReviewCommitteeVM
{
    public int Id { get; set; }
    public string ReviewCommitteeName { get; set; }
    public int CompetitionId { get; set; }
    public string CompetitionName { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public ICollection<ReviewBoardMemberVM> ReviewBoardMembers { get; set; }
}

public class ReviewBoardMemberVM : ReviewerVM
{
    public bool IsStatus { get; set; }
    public string Description { get; set; }
}





