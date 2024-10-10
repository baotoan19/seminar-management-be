using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Acceptances")]
    public class Acceptance: BaseEntity
    {
        public string Name { get; set; }
        [ForeignKey("Topics")]
        public int TopicId { get; set; }
        public bool IsStatus { get; set; }
        public virtual Topic Topic { get; set; }
    }
}