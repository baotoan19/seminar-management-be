namespace Seminar.APPLICATION.Models;
public class PostVM
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required DateTime DateUpLoad { get; set; }
    public required int OrganizerId { get; set; }
    public string? OrganizerName { get; set; }
}