using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.StatisticDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticController : ControllerBase
{
    private readonly IStatisticService _statisticService;
    public StatisticController(IStatisticService statisticService)
    {
        _statisticService = statisticService;
    }

    [HttpPost("statistics")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> GetStatisticsByOrganizer([FromBody] StatisticsFilterDto filterDto)
    {
        var result = await _statisticService.GetStatisticsByOrganizer(filterDto);
        return Ok(new BaseResponse<StatisticsVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Lấy thống kê theo id ban tổ chức thành công",
            data: result));
    }
}
