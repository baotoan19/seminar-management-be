using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Review_Assignment : BaseEntity
    {
        [ForeignKey("Organizer")]
        public int? OrganizerId { get; set; }
        [ForeignKey("Artical")]
        public int? ArticalId { get; set; }
        [ForeignKey("Reviewer")]
        public int? ReviewerId { get; set; }
        public bool Status { get; set; }
        public virtual Organizer Organizer { get; set; }
        public virtual Artical Artical { get; set; }
        public virtual Reviewer Reviewer { get; set; }
    }
}
