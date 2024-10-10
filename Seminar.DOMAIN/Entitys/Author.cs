using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Authors")]
    public class Author: BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? NumberPhone { get; set; }
        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public string? InternalCode { get; set; }
        [ForeignKey("Faculties")]
        public int? FacultyId { get; set; }
        public virtual Account Account { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Author_Articel> Author_Articlas { get; set; } = new List<Author_Articel>();
        public virtual ICollection<RegistrationForm> RegistrationForms { get; set; } = new List<RegistrationForm>();
        public virtual ICollection<Author_Topic> Author_Topics { get; set; } = new List<Author_Topic>();
    }
}
