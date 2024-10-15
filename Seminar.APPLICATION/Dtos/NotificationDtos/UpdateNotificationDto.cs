using System.ComponentModel.DataAnnotations;
namespace Seminar.APPLICATION.Dtos.NotificationDtos;

public class UpdateNotificationDto
{
    [Required(ErrorMessage = "Notification ID is required")]
    public bool Status { get; set; }
}