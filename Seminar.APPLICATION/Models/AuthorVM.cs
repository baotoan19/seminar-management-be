namespace Seminar.APPLICATION.Models;

public class AuthorVM : UserVM
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? NumberPhone { get; set; }
    public int? FacultyId { get; set; }
    public string? FacultyName { get; set; }
    public string? InternalCode { get; set; }

}