namespace Seminar.APPLICATION.Models;

public class NotificationVM
{
    public int Id { get; set; }
    public string NotificationContent { get; set; }
    public DateTime NotificationDate { get; set; }
    public int RecevierId { get; set; }
    public string RecevierName { get; set; }
    public string RecevierEmail { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public int NotificationTypeId { get; set; }
    public string NotificationTypeName { get; set; }
    public int TargetId { get; set; }
    public bool Status { get; set; }
}