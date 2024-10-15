using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Review_Forms")]
    public class Review_Form : BaseEntity
    {
        public string? Content { get; set; }
        [ForeignKey("History_Update_ResearchTopics")]
        public int? HistoryId { get; set; }
        [ForeignKey("Reviewers")]
        public int? ReviewerId { get; set; }
        [ForeignKey("Concludes")]
        public int? ConcludeId { get; set; }
        public DateTime? Date_Upload { get; set; }
        public virtual History_Update_ResearchTopic History_Update_ResearchTopic { get; set; }
        public virtual Reviewer Reviewer { get; set; }
        public virtual Conclude Conclude { get; set; }
    }
}
