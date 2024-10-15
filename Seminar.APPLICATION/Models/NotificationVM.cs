namespace Seminar.APPLICATION.Models;

public class NotificationVM
{
    public string NotificationContent { get; set; }
    public DateTime NotificationDate { get; set; }
    public int RecevierId { get; set; }
    public int SenderId { get; set; }
    public int NotificationTypeId { get; set; }
    public string NotificationTypeName { get; set; }
    public int TargetId { get; set; }
    public bool Status { get; set; }
}