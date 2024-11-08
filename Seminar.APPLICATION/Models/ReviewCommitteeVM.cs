namespace Seminar.APPLICATION.Models;

public class ReviewCommitteeVM 
{
    public int Id { get; set; }
    public string ReviewCommitteeName { get; set; }
    public string CompetitionName { get; set; }
    public int CompetitionId { get; set; }
}

public class ReviewCommitteeDto
{
    public int Id { get; set; }
    public string ReviewCommitteeName { get; set; }
}


