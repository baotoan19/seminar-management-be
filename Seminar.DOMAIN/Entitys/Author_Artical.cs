using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Author_Artical:BaseEntity
    {
        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        [ForeignKey("Artical")]
        public int? ArticalId { get; set; }
        public string? Role { get; set; }
        public virtual Author Author { get; set; }
        public virtual Artical Artical { get; set; }
    }
}
