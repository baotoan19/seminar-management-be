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
    public class Notification : BaseEntity
    {
        [ForeignKey("Artical")]
        public int? ArticalId { get; set; }
        [StringLength(255)]
        public string? NotificationContent { get; set; }
        public DateTime? NotificationDate { get; set; }
        public int? RecevierId { get; set; }
        public int? SenderId { get; set; }
        public bool Status { get; set; }
        public virtual Artical Artical { get; set; }
    }
}
