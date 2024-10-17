namespace Seminar.APPLICATION.Models;
public class ReviewerVM : UserVM
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string NumberPhone { get; set; }
    public string AcademicDegree { get; set; }
    public string AcademicRank { get; set; }
    public int FacultyId { get; set; }
    public string FacultyName { get; set; }
}