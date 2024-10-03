namespace Seminar.APPLICATION.Models;

public class ReviewAssignmentVM
{
    public int Id { get; set; }
    public int OrganizerId { get; set; }
    public string? OrganizerName { get; set; }
    public int ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public int ArticalId { get; set; }
    public string ArticalTitle { get; set; }
    public bool Status { get; set; }
}