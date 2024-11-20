using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.EmailDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send-feedback")]
    public async Task<IActionResult> SendFeedBackEmailAsync(EmailFeedBackDto emailFeedBackDto)
    {
        await _emailService.SendFeedBackEmail(emailFeedBackDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: null,
            message: "Gửi email phản biện đề tài thành công"));
    }

    [HttpPost("send-system")]
    public async Task<IActionResult> SendSystemEmailAsync(EmailSystemDto emailSystemDto)
    {
        await _emailService.SendSystemEmail(emailSystemDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: null,
            message: "Gửi email thành công"));
    }
}
