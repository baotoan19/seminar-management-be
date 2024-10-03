using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.DOMAIN.Entitys;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}, {CLAIMS_VALUES.ROLE_TYPE.REVIEWER}")]
public class ConcludeController : ControllerBase
{
    private readonly IConcludeService _concludeService;

    public ConcludeController(IConcludeService concludeService)
    {
        _concludeService = concludeService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllAsync()
    {
        IList<ConcludeVM> concludes = await _concludeService.GetAllAsync();
        return Ok(new BaseResponse<IList<ConcludeVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: concludes));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        ConcludeVM conclude = await _concludeService.GetByIdAsync(id);
        return Ok(new BaseResponse<ConcludeVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: conclude));
    }
    
}