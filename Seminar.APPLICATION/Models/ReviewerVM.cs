namespace Seminar.APPLICATION.Models;
public class ReviewerVM : UserVM
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? NumberPhone { get; set; }
    public string? AcademicDegree { get; set; }
    public string? AcademicRank { get; set; }
    public int? DisciplineId { get; set; }
    public string? DisciplineName { get; set; }
    public int? ReviewCommitteeId { get; set; }
    public string? ReviewCommitteeName { get; set; }
}