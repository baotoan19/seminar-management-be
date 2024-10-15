using Seminar.CORE.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace Seminar.DOMAIN.Entitys
{
    [Table("Author_Articles")]
    public class Author_Article:BaseEntity
    {
        [ForeignKey("Authors")]
        public int? AuthorId { get; set; }
        [ForeignKey("Articles")]
        public int? ArticleId { get; set; }
        public string? RoleName { get; set; }
        public virtual Author Author { get; set; }
        public virtual Article Article { get; set; }
    }
}
