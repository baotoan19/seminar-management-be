using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.HistoryArticasDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HistoryUpdateArticalsController : ControllerBase
{
    private readonly IHistoryArticalsService _historyUpdateArticalsService;

    public HistoryUpdateArticalsController(IHistoryArticalsService historyUpdateArticalsService)
    {
        _historyUpdateArticalsService = historyUpdateArticalsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHistoryUpdateArticalsByIdAsync(int id)
    {
        HistoryArticalsVM historyArticals = await _historyUpdateArticalsService.GetHistoryUpdateArticalsByIdAsync(id);
        return Ok(new BaseResponse<HistoryArticalsVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: historyArticals));
    }

    [HttpPost]
    public async Task<IActionResult> CreateHistoryUpdateArticalsAsync(CreateHistoryArticalsDto createHistoryArticalsDto)
    {
        await _historyUpdateArticalsService.CreateHistoryUpdateArticalsAsync(createHistoryArticalsDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Create history update artical success"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHistoryUpdateArticalsAsync(int id)
    {
        await _historyUpdateArticalsService.DeleteHistoryUpdateArticalsAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Delete history update artical success"));
    }
}