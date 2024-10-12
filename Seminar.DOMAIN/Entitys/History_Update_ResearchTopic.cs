using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Seminar.CORE.Base;

namespace Seminar.DOMAIN.Entitys
{
    public class History_Update_ResearchTopic : BaseEntity
    {
        [ForeignKey("ResearchTopics")]
        public int? ResearchTopicId { get; set; }
        [Required]
        [StringLength(255)]
        public string? NewFilePath { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string? Summary { get; set; }
        public virtual ResearchTopic ResearchTopic { get; set; }
        public virtual ICollection<Review_Form> Review_Forms { get; set; } = new List<Review_Form>();
    }
}
