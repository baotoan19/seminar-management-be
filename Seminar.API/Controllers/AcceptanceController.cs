using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.AcceptanceDtos;
using Seminar.APPLICATION.Dtos.ReviewAcceptanceDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AcceptanceController : ControllerBase
{
    private readonly IAcceptanceService _acceptanceService;
    public AcceptanceController(IAcceptanceService acceptanceService)
    {
        _acceptanceService = acceptanceService;
    }
    [HttpGet("all-acceptances")]
    public async Task<IActionResult> GetAllAcceptances(int index = 1, int pageSize = 10, int idSearch = 0, string nameSearch = "", int facultyAcceptedStatus = 3, int acceptedForPublicationStatus = 3, int competitionId = 0, int facultyId = 0)
    {
        var result = await _acceptanceService.GetAllAcceptances(index, pageSize, idSearch, nameSearch, facultyAcceptedStatus, acceptedForPublicationStatus, competitionId, facultyId);
        return Ok(new BaseResponse<PaginatedList<AcceptanceVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Lấy tất cả các nghiệm thu thành công!",
            data: result));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAcceptanceById(int id)
    {
        var result = await _acceptanceService.GetAcceptanceById(id);
        return Ok(new BaseResponse<AcceptanceVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Lấy nghiệm thu theo id thành công!",
            data: result));
    }
    [HttpPost]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> CreateAcceptance(CreateAcceptanceDto dto)
    {
        await _acceptanceService.CreateAcceptance(dto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Tạo nghiệm thu thành công!",
            data: null));
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> DeleteAcceptance(int id)
    {
        await _acceptanceService.DeleteAcceptance(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Xóa nghiệm thu thành công!",
            data: null));
    }
    [HttpPost("review")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> CreateReviewAcceptance(CreateReviewAcceptanceDto dto)
    {
        await _acceptanceService.CreateReviewAcceptance(dto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Tạo phản biện nghiệm thu thành công!",
            data: null));
    }
    [HttpPatch("update-for-publication/{id}")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
    public async Task<IActionResult> UpdateAcceptanceForPublication(int id, UpdateAcceptanceForPublicationDto dto)
    {
        await _acceptanceService.UpdateAcceptanceForPublication(id, dto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Cập nhật nghiệm thu để public thành công!",
            data: null));
    }
}
