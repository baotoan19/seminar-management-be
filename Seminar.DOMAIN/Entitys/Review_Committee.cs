using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seminar.CORE.Base;

namespace Seminar.DOMAIN.Entitys
{
    public class Review_Committee : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string ReviewCommitteeName { get; set; }
        [ForeignKey("Conference")]
        public int? ConferenceId { get; set; }
        public virtual Conference Conference { get; set; }
        public virtual ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    }
}
