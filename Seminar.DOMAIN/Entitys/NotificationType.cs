using System.ComponentModel.DataAnnotations.Schema;
using Seminar.CORE.Base;

namespace Seminar.DOMAIN.Entitys
{
    [Table("NotificationTypes")]
    public class NotificationType: BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}

