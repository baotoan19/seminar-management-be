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
    [Table("Posts")]
    public class Post : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        public string? Content { get; set; }
        public DateTime? DateUpload { get; set; }
        [ForeignKey("Organizers")]
        public int? OrganizerId { get; set; }
        public virtual Organizer Organizers { get; set; }
    }
}
