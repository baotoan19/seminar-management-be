using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Seminar.CORE.Base;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Reviewers")]
    public class Reviewer : BaseEntity
    {
        public string? Name { get; set; }
        public string? NumberPhone { get; set; }
        [StringLength(255)]
        public string? AcademicRank { get; set; }
        public string? AcademicDegree { get; set; }
        [ForeignKey("Faculties")]
        public int? FacultyId { get; set; }
        [ForeignKey("Accounts")]
        public int AccountId { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual Account Account { get; set; }
        public virtual ICollection<Review_Assignment> Review_Assignments { get; set; } = new List<Review_Assignment>();
        public virtual ICollection<Review_Form> Review_Forms { get; set; } = new List<Review_Form>();
        public virtual ICollection<Review_Board_Member> Review_Board_Members { get; set; } = new List<Review_Board_Member>();
    }
}
