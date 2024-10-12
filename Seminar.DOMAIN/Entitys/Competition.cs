using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seminar.CORE.Base;

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
        [StringLength(255)]
        [ForeignKey("Organizers")]
        public int? OrganizerId { get; set; }
        public virtual ICollection<ResearchTopic> ResearchTopics { get; set; } = new List<ResearchTopic>();
        public virtual Organizer Organizer { get; set; }
        public virtual ICollection<Review_Committee> Review_Committees { get; set; } = new List<Review_Committee>();
        public virtual ICollection<RegistrationForm> RegistrationForms { get; set; } = new List<RegistrationForm>();
    }
}
