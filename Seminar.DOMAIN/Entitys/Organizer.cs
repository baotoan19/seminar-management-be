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
    [Table("Organizers")]
    public class Organizer : BaseEntity
    {
        public string? Name { get; set; }
        public string? NumberPhone { get; set; }
        [StringLength(255)]
        public string? Description { get; set; }
        [ForeignKey("Accounts")]
        public int AccountId { get; set; }
        [ForeignKey("Faculties")]
        public int? FacultyId { get; set; }
        public virtual Account Account { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Conference> Conferences { get; set; } = new List<Conference>();
        public virtual ICollection<Review_Assignment> Review_Assignments { get; set; } = new List<Review_Assignment>();
    }
}
