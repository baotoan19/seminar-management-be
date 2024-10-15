using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Seminar.DOMAIN.Entitys
{
    [Table("Disciplines")]
    public class Discipline : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string DisciplineName { get; set; }
        public virtual ICollection<ResearchTopic> ResearchTopics { get; set; } = new List<ResearchTopic>();
    }
}
