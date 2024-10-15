using Seminar.APPLICATION.Dtos.NotificationDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface INotificationService
{
    Task<NotificationVM> GetNotificationByIdAsync(int id);
    Task CreateNotificationAsync(CreateNotificationDto createNotificationDto);
    Task UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto);
    Task<List<NotificationVM>> GetAllNotificationByReceiverAsync(int receiverId);
    Task DeleteNotificationAsync(int id);
}
