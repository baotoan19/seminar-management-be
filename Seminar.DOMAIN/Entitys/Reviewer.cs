﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seminar.CORE.Base;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Reviewers")]
    public class Reviewer : BaseEntity
    {
        public string? Name { get; set; }
        public string? NumberPhone { get; set; }
        [StringLength(255)]
        public string? AcademicRank { get; set; }
        public string? AcademicDegree { get; set; }
        [ForeignKey("Disciplines")]
        public int? DisciplineId { get; set; }
        [ForeignKey("Review_Committees")]
        public int? ReviewCommitteeId { get; set; }
        [ForeignKey("Accounts")]
        public int AccountId { get; set; }
        public virtual Discipline Discipline { get; set; }
        public virtual Review_Committee Review_Committee { get; set; }
        public virtual Account Account { get; set; }
        public virtual ICollection<Review_Assignment> Review_Assignments { get; set; } = new List<Review_Assignment>();
        public virtual ICollection<Review_Form> Review_Forms { get; set; } = new List<Review_Form>();
    }
}
