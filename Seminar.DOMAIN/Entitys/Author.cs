using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;


namespace Seminar.DOMAIN.Entitys
{
    [Table("Authors")]
    public class Author: BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? NumberPhone { get; set; }
        [ForeignKey("Accounts")]
        public int? AccountId { get; set; }
        public string? InternalCode { get; set; }
        [ForeignKey("Faculties")]
        public int? FacultyId { get; set; }
        public virtual Account Account { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Author_Article> Author_Articles { get; set; } = new List<Author_Article>();
        public virtual ICollection<RegistrationForm> RegistrationForms { get; set; } = new List<RegistrationForm>();
        public virtual ICollection<Author_ResearchTopic> Author_ResearchTopics { get; set; } = new List<Author_ResearchTopic>();
    }
}
