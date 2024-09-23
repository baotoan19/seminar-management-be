namespace Seminar.APPLICATION.Models;
public class OrganizerVM : UserVM
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? NumberPhone { get; set; }
    public string? Description { get; set; }
    public int? FacultyId { get; set; }
    public string? FacultyName { get; set; }
}