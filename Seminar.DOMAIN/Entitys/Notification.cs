using System.ComponentModel.DataAnnotations.Schema;
using Seminar.CORE.Base;

namespace Seminar.DOMAIN.Entitys
{
    [Table("Notifications")]
    public class Notification : BaseEntity
    {
        [ForeignKey("NotificationTypes")]
        public int NotificationTypeId { get; set; }
        public int TargetId { get; set; }
        public string NotificationContent { get; set; }
        public DateTime NotificationDate { get; set; }
        public int RecevierId { get; set; }
        public int SenderId { get; set; }
        public bool Status { get; set; }
        public virtual NotificationType NotificationTypes { get; set; }
    }
}
