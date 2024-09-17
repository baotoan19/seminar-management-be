using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Discipline : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string DisciplineName { get; set; }
        public virtual ICollection<Artical> Articals { get; set; } = new List<Artical>();
        public virtual ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    }
}
