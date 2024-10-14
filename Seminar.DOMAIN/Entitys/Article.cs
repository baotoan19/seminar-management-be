using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Seminar.DOMAIN.Entitys
{
    [Table("Articles")]
    public class Article : BaseEntity
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
        [ForeignKey("Disciplines")]
        public int? DisciplineId { get; set; }
        public virtual Discipline Discipline { get; set; }
        public virtual ICollection<Author_Article> Author_Articles { get; set; } = new List<Author_Article>();
        public virtual ICollection<ResearchTopic> ResearchTopics { get; set; } = new List<ResearchTopic>();
    }
}
