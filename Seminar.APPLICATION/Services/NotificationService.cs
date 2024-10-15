using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.NotificationDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<NotificationVM> GetNotificationByIdAsync(int id)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        Notification notification = await _unitOfWork.GetRepository<Notification>().Entities
            .Include(n => n.NotificationTypes)
            .FirstOrDefaultAsync(n => n.Id == id) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Notification not found!");
        if (notification.RecevierId.ToString() != userId)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not authorized to view this notification.");
        }
        NotificationVM notificationVM = _mapper.Map<NotificationVM>(notification);
        notificationVM.NotificationTypeName = notification.NotificationTypes?.Name ?? "";
        return notificationVM;
    }

    public async Task CreateNotificationAsync(CreateNotificationDto createNotificationDto)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        NotificationType notificationTypes = await _unitOfWork.GetRepository<NotificationType>().Entities.FirstOrDefaultAsync(nt => nt.Id == createNotificationDto.NotificationTypeId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Notification type not found!");
        Notification notification = _mapper.Map<Notification>(createNotificationDto);
        notification.SenderId = int.Parse(userId);
        await _unitOfWork.GetRepository<Notification>().InsertAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        Notification notification = await _unitOfWork.GetRepository<Notification>().GetByIdAsync(id);
        _mapper.Map(updateNotificationDto, notification);
        notification.UpdatedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<NotificationVM>> GetAllNotificationByReceiverAsync(int receiverId)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        if (receiverId.ToString() != userId)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not authorized to view these notifications.");
        }
        List<Notification> notifications = await _unitOfWork.GetRepository<Notification>().Entities
            .Include(n => n.NotificationTypes)
            .Where(n => n.RecevierId == receiverId && n.DeletedAt == null)
            .ToListAsync();
        List<NotificationVM> notificationVMs = _mapper.Map<List<NotificationVM>>(notifications);
        foreach (NotificationVM notificationVM in notificationVMs)
        {
            notificationVM.NotificationTypeName = notifications.FirstOrDefault(n => n.Id == notificationVM.Id)?.NotificationTypes?.Name ?? "";
        }
        return notificationVMs;
    }

    public async Task DeleteNotificationAsync(int id)
    {
        Notification notification = await _unitOfWork.GetRepository<Notification>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Notification not found!");
        notification.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Notification>().UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }


}
