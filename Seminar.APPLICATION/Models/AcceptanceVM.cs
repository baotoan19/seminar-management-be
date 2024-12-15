namespace Seminar.APPLICATION.Models;

public class AcceptanceVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateAcceptance { get; set; }
    public int FacultyAcceptedStatus { get; set; }
    public int AcceptedForPublicationStatus { get; set; }
    public ResearchTopicVM ResearchTopic { get; set; }
    public ICollection<ReviewAcceptanceVM> ReviewAcceptances { get; set; }
}

public class ReviewAcceptanceVM
{
    public int Id { get; set; }
    public string Description { get; set; }
    public bool IsAccepted { get; set; }
    public OrganizerVM Organizer { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
