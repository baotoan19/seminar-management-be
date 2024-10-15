using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Seminar.DOMAIN.Entitys
{
    [Table("Concludes")]
    public class Conclude : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Result { get; set; }
        public virtual ICollection<Review_Form> Review_Forms { get; set; } = new List<Review_Form>();
    }
}
