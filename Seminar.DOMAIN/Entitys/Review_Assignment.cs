using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace Seminar.DOMAIN.Entitys
{
    [Table("Review_Assignments")]
    public class Review_Assignment : BaseEntity
    {
        [ForeignKey("Organizers")]
        public int? OrganizerId { get; set; }
        [ForeignKey("ResearchTopics")]
        public int ResearchTopicId { get; set; }
        [ForeignKey("Reviewers")]
        public int? ReviewerId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool Status { get; set; }
        public virtual Organizer Organizer { get; set; }
        public virtual ResearchTopic ResearchTopic { get; set; }
        public virtual Reviewer Reviewer { get; set; }
    }
}
