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
    public class History_Update_Topic : BaseEntity
    {
        [ForeignKey("Topic")]
        public int? TopicId { get; set; }
        [Required]
        [StringLength(255)]
        public string? NewFilePath { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string? Summary { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual ICollection<Review_Form> Review_Forms { get; set; } = new List<Review_Form>();
    }
}
