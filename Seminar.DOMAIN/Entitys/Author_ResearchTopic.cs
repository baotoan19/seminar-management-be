using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;


namespace Seminar.DOMAIN.Entitys
{
    [Table("Author_ResearchTopics")]
    public class Author_ResearchTopic: BaseEntity
    {
        [ForeignKey("ResearchTopics")]
        public int ResearchTopicId { get; set; }
        [ForeignKey("Authors")]
        public int AuthorId { get; set; }
        public string RoleName { get; set; }
        public virtual ResearchTopic ResearchTopic { get; set; }
        public virtual Author Author { get; set; }
    }
}