using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class RegistrationForm : BaseEntity
    {

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        [ForeignKey("Conference")]
        public int ConferenceId { get; set; }
        public string? FilePath { get; set; }
        public int IsAccepted { get; set; }
        public virtual Author Author { get; set; }
        public virtual Conference Conference { get; set; }
    }
}
