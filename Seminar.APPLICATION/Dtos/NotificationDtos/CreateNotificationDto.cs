using System.ComponentModel.DataAnnotations;
namespace Seminar.APPLICATION.Dtos.NotificationDtos;

public class CreateNotificationDto
{
    [Required(ErrorMessage = "Notification content is required")]
    public string NotificationContent {get; set;}
    [Required(ErrorMessage = "Notification date is required")]
    public DateTime NotificationDate {get; set;}
    [Required(ErrorMessage = "Receiver ID is required")]
    public int RecevierId {get; set;}
    [Required(ErrorMessage = "Sender ID is required")]
    public int SenderId {get; set;}
    [Required(ErrorMessage = "Notification type ID is required")]
    public int NotificationTypeId {get; set;}
    [Required(ErrorMessage = "Target ID is required")]
    public int TargetId {get; set;}
}