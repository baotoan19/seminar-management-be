using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Author_Articels")]
    public class Author_Articel:BaseEntity
    {
        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        [ForeignKey("Articel")]
        public int? ArticelId { get; set; }
        public string? RoleName { get; set; }
        public virtual Author Author { get; set; }
        public virtual Articel Articel { get; set; }
    }
}
