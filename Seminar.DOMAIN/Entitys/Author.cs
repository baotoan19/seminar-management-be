using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Author: BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? NumberPhone { get; set; }
        [ForeignKey("Account")]
        public int? AccountId { get; set; }
        public string? InternalCode { get; set; }
        [ForeignKey("Faculty")]
        public int? FacultyId { get; set; }
        public virtual Account Account { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Author_Artical> Author_Articlas { get; set; } = new List<Author_Artical>();
        public virtual ICollection<RegistrationForm> RegistrationForms { get; set; } = new List<RegistrationForm>();
    }
}
