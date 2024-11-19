using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.GoogleAiDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoogleAIController : ControllerBase
{
    private readonly IGoogleAIService _googleAIService;

    public GoogleAIController(IGoogleAIService googleAIService)
    {
        _googleAIService = googleAIService;
    }

    [HttpPost("summarize")]
    public async Task<IActionResult> SummarizeText([FromBody] string text, [FromQuery] int maxLength = 150, [FromQuery] string language = "vi")
    {
        var result = await _googleAIService.SummarizeTextAsync(text, maxLength, language);
        return Ok(new BaseResponse<SummaryResultDto>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Tóm tắt văn bản thành công",
            data: result));
    }
}
