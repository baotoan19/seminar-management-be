using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Acceptances")]
    public class Acceptance : BaseEntity
    {
        public string Name { get; set; }
        [ForeignKey("ResearchTopics")]
        public int ResearchTopicId { get; set; }
        public double Budget { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? FinalFilePath { get; set; }
        public bool IsAcceptedForPublication { get; set; }
        public virtual ResearchTopic ResearchTopic { get; set; }
        public virtual ICollection<Review_Acceptance> Review_Acceptances { get; set; } = new List<Review_Acceptance>();
    }
}