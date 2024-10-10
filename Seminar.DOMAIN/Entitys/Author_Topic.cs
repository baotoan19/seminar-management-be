using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Author_Topics")]
    public class Author_Topic: BaseEntity
    {
        [ForeignKey("Topics")]
        public int TopicId { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public string RoleName { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Author Author { get; set; }
    }
}