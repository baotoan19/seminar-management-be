using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Roles")]
    public class Role : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string RoleName { get; set; }
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
