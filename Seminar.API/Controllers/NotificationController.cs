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
            message: "Lấy thông báo theo id thành công",
            data: notification));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotificationByReceiverAsync()
    {
        List<NotificationVM> notifications = await _notificationService.GetAllNotificationByReceiverAsync();
        return Ok(new BaseResponse<List<NotificationVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Lấy tất cả thông báo theo người nhận thành công",
            data: notifications));
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
    {
        await _notificationService.CreateNotificationAsync(createNotificationDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Tạo thông báo thành công",
            data: null));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto)
    {
        await _notificationService.UpdateNotificationAsync(id, updateNotificationDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Cập nhật thông báo thành công",
            data: null));
    }

    [HttpPatch("mark-all-as-read")]
    public async Task<IActionResult> UpdateMarkAllNotificationAsReadAsync()
    {
        await _notificationService.UpdateMarkAllNotificationAsReadAsync();
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Thông báo đã được đánh dấu đã đọc thành công",
            data: null));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotificationAsync(int id)
    {
        await _notificationService.DeleteNotificationAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Xóa thông báo thành công",
            data: null));
    }


    
    


}
