using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Seminar.DOMAIN.Entitys
{
    [Table("Accounts")]
    public class Account : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string Password { get; set; }
        [ForeignKey("Role")]
        public int? RoleId { get; set; }
        public bool Status { get; set; }
        public virtual Role Role { get; set; }
        public virtual Author Author { get; set; }
        public virtual Reviewer Reviewer { get; set; }
        public virtual Organizer Organizer { get; set; }
    }
}
