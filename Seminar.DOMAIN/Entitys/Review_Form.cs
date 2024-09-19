using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Review_Form : BaseEntity
    {
        public string? Content { get; set; }
        [ForeignKey("History")]
        public int? HistoryId { get; set; }
        [ForeignKey("Reviewer")]
        public int? ReviewerId { get; set; }
        [ForeignKey("Conclude")]
        public int? ConcludeId { get; set; }
        public DateTime? Date_Upload { get; set; }
        public virtual History_Update_Artical History { get; set; }
        public virtual Reviewer Reviewer { get; set; }
        public virtual Conclude Conclude { get; set; }
    }
}
