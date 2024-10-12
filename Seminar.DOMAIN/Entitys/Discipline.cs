using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Disciplines")]
    public class Discipline : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string DisciplineName { get; set; }
        public virtual ICollection<ResearchTopic> ResearchTopics { get; set; } = new List<ResearchTopic>();
        public virtual ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    }
}
