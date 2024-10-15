using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.NotificationDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationByIdAsync(int id)
    {
        NotificationVM notification = await _notificationService.GetNotificationByIdAsync(id);
        return Ok(new BaseResponse<NotificationVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Notification retrieved successfully!",
            data: notification));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotificationByReceiverAsync(int receiverId)
    {
        List<NotificationVM> notifications = await _notificationService.GetAllNotificationByReceiverAsync(receiverId);
        return Ok(new BaseResponse<List<NotificationVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Notifications retrieved successfully!",
            data: notifications));
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
    {
        await _notificationService.CreateNotificationAsync(createNotificationDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Notification created successfully!"));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto)
    {
        await _notificationService.UpdateNotificationAsync(id, updateNotificationDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Notification updated successfully!"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotificationAsync(int id)
    {
        await _notificationService.DeleteNotificationAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Notification deleted successfully!"));
    }


    
    


}
