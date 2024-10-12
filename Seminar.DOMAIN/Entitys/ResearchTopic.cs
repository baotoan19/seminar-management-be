using Seminar.CORE.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DOMAIN.Entitys
{
    [Table("ResearchTopics")]
    public class ResearchTopic: BaseEntity
    {
        public string NameTopic { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateUpLoad { get; set; }
        public double Budget { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public string AchievedResults { get; set; }
        public bool IsAcceptanceStatus { get; set; }
        public bool IsReviewStatus { get; set; }
        public string? ProductFilePath { get; set; }
        public string? ReportFilePath {get; set;}
        public string? FinalFilePath {get; set;}
        [ForeignKey("Articles")]
        public int? ArticleId {get; set;}
        [ForeignKey("Disciplines")]
        public int DisciplineId { get; set; }
        [ForeignKey("Competitions")]
        public int CompetitionId { get; set; }
        public virtual Acceptance Acceptance { get; set; }
        public virtual ICollection<Author_ResearchTopic> Author_ResearchTopics { get; set; } = new List<Author_ResearchTopic>();
        public virtual ICollection<History_Update_ResearchTopic> History_Update_ResearchTopics { get; set; } = new List<History_Update_ResearchTopic>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<Review_Assignment> Review_Assignments { get; set; } = new List<Review_Assignment>();
        public virtual Competition Competitions { get; set; }
        public virtual Discipline Disciplines { get; set; }
    }
}