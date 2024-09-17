using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Artical : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        public string? Text { get; set; }
        [StringLength(255)]
        public string? KeyWord { get; set; }
        public string? FilePath { get; set; }
        public DateTime? DateUpload { get; set; }
        [ForeignKey("Discipline")]
        public int? DisciplineId { get; set; }
        [ForeignKey("Conference")]
        public int? ConferenceId { get; set; }
        [ForeignKey("Proceeding")]
        public int? ProceedingId { get; set; }
        public virtual Discipline Discipline { get; set; } = new Discipline();
        public virtual Conference Conference { get; set; }
        public virtual ICollection<Review_Assignment> Review_Assignments { get; set; } = new List<Review_Assignment>();
        public virtual ICollection<Review_Form> Review_Forms { get; set; } = new List<Review_Form>();
        public virtual ICollection<History_Update_Artical> History_Update_Articlas { get; set; } = new List<History_Update_Artical>();
        public virtual ICollection<Author_Artical> Author_Articlas { get; set; } = new List<Author_Artical>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual Proceeding Proceeding { get; set; }
    }
}
