using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    public class Proceeding : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? ImagePath { get; set; }
        public virtual ICollection<Artical> Articals { get; set; } = new List<Artical>();
    }
}
