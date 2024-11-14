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
        public DateTime? DateAcceptance { get; set; }
        public int FacultyAcceptedStatus { get; set; }
        public int AcceptedForPublicationStatus { get; set; }
        public virtual ResearchTopic ResearchTopic { get; set; }
        public virtual ICollection<Review_Acceptance> Review_Acceptances { get; set; } = new List<Review_Acceptance>();
    }
}