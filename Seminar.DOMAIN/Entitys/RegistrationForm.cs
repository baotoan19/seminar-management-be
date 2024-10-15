using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seminar.DOMAIN.Entitys
{
    [Table("RegistrationForms")]
    public class RegistrationForm : BaseEntity
    {

        [ForeignKey("Authors")]
        public int AuthorId { get; set; }
        [ForeignKey("Competitions")]
        public int CompetitionId { get; set; }
        public string? FilePath { get; set; }
        public int IsAccepted { get; set; }
        public virtual Author Author { get; set; }
        public virtual Competition Competition { get; set; }
    }
}
