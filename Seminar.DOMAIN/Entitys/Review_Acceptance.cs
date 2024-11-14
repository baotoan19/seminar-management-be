using System.ComponentModel.DataAnnotations.Schema;
using Seminar.CORE.Base;
namespace Seminar.DOMAIN.Entitys;
[Table("Review_Acceptances")]
public class Review_Acceptance:BaseEntity
{
    [ForeignKey("Acceptances")]
    public int AcceptanceId { get; set; }
    [ForeignKey("Organizers")]
    public int OrganizerId { get; set; }
    public bool IsAccepted { get; set; }
    public string Description { get; set; }
    public virtual Acceptance Acceptance { get; set; }
    public virtual Organizer Organizer { get; set; }
}