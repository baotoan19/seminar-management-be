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
    [Table("Articels")]
    public class Articel : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        public string? Description { get; set; }
        [StringLength(255)]
        public string? KeyWord { get; set; } 
        public string? FilePath { get; set; }
        public DateTime? DateUpload { get; set; }
        public bool IsStatus { get; set; }
        [ForeignKey("Discipline")]
        public int? DisciplineId { get; set; }
        public virtual Discipline Discipline { get; set; }
        public virtual ICollection<Author_Articel> Author_Articlas { get; set; } = new List<Author_Articel>();
    }
}
