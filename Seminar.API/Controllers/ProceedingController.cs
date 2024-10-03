using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ProceedingsDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProceedingController : ControllerBase
{
    private readonly IProceedingService _proceedingService;

    public ProceedingController(IProceedingService proceedingService)
    {
        _proceedingService = proceedingService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProceedingByIdAsync(int id)
    {
        ProceedingVM proceeding = await _proceedingService.GetProceedingByIdAsync(id);
        return Ok(new BaseResponse<ProceedingVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: proceeding));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProceedingAsync(CUProceedingDto proceedingDto)
    {
        await _proceedingService.CreateProceedingAsync(proceedingDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Create proceeding successfully!"));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateProceedingAsync(int id, CUProceedingDto proceedingDto)
    {
        await _proceedingService.UpdateProceedingAsync(id, proceedingDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Update proceeding successfully!"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProceedingAsync(int id)
    {
        await _proceedingService.DeleteProceedingAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Delete proceeding successfully!"));
    }
}