using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Faculty : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string FacultyName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Organizer> Organizers { get; set; } = new List<Organizer>();
        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
    }
}
