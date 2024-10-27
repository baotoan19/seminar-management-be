using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Seminar.CORE.Base;
using Seminar.DOMAIN.Enum;
namespace Seminar.DOMAIN.Entitys
{
    [Table("Competitions")]
    public class Competition : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string CompetitionName { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? DateEndSubmit {get; set;}
        [ForeignKey("Organizers")]
        public int? OrganizerId { get; set; }
        public string Description { get; set; }
        public string Destination { get; set; }
        [NotMapped]
        public CompetitionStatus Status
        {
            get
            {
                DateTime now = DateTime.Now;
                if (now < DateStart)
                    return CompetitionStatus.Upcoming;
                if (now >= DateStart && now <= DateEnd)
                    return CompetitionStatus.Active;
                return CompetitionStatus.Completed;
            }
        }
        public virtual Organizer Organizer { get; set; }
        public virtual ICollection<ResearchTopic> ResearchTopics { get; set; } = new List<ResearchTopic>();
        public virtual ICollection<Review_Committee> Review_Committees { get; set; } = new List<Review_Committee>();
        public virtual ICollection<RegistrationForm> RegistrationForms { get; set; } = new List<RegistrationForm>();
    }
}
