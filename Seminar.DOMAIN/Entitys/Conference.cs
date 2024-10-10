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
    [Table("Conferences")]
    public class Conference : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string ConferenceName { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        [StringLength(255)]
        public string? Destination { get; set; }
        [ForeignKey("Organizers")]
        public int? OrganizerId { get; set; }
        public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public virtual Organizer Organizer { get; set; }
        public virtual ICollection<Review_Committee> Review_Committees { get; set; } = new List<Review_Committee>();
        public virtual ICollection<RegistrationForm> RegistrationForms { get; set; } = new List<RegistrationForm>();
    }
}
