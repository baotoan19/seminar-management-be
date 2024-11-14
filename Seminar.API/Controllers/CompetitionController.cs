using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.CompetitionDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.API.Controllers;

[Route("api/competitions")]
[ApiController]

public class CompetitionController : ControllerBase
{
    private readonly ICompetitionService _competitionService;
    public CompetitionController(ICompetitionService competitionService)
    {
        _competitionService = competitionService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetCompetitionByIdAsync(int id)
    {
        CompetitionVM competition = await _competitionService.GetCompetitionByIdAsync(id);
        return Ok(new BaseResponse<CompetitionVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: competition));
    }

    [HttpGet("organizer")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> GetAllCompetitionByOrganizerIdAsync(int index = 1, int pageSize = 8, string nameSearch = "")
    {
        PaginatedList<CompetitionVM> competitions = await _competitionService.GetAllCompetitionByOrganizerIdAsync(index, pageSize,nameSearch);
        return Ok(new BaseResponse<PaginatedList<CompetitionVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: competitions));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCompetitionAsync(int index = 1, int pageSize = 8, string nameSearch = "", string organizerName = "")
    {
        PaginatedList<CompetitionVM> competitions = await _competitionService.GetAllCompetitionAsync(index, pageSize, nameSearch, organizerName);
        return Ok(new BaseResponse<PaginatedList<CompetitionVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: competitions));
    }

    [HttpPost()]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> CreateCompetitionAsync(CreateCompetitionDto createCompetitionDto)
    {
        await _competitionService.CreateCompetitionAsync(createCompetitionDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Create competition successfully!"
            ));
        
    }

    [HttpPatch()]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> UpdateCompetitionAsync(int id, UpdateCompetitionDto updateCompetitionDto)
    {
        await _competitionService.UpdateCompetitionAsync(id, updateCompetitionDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update competition successfully!"
            ));
    }

    [HttpDelete()]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> DeleteCompetitionAsync(int id)
    {
        await _competitionService.DeleteCompetitionAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Delete competition successfully!"
            ));
    }

    [HttpPatch("date-end")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> UpdateDateEndCompetitionAsync(int id, UpdateDateEndCompetitionDto updateDateEndCompetitionDto)
    {
        await _competitionService.UpdateDateEndCompetitionAsync(id, updateDateEndCompetitionDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update date end competition successfully!"));
    }

    [HttpPatch("date-submit")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> UpdateDateSubmitCompetitionAsync(int id, UpdateDateSubmitCompetitionDto updateDateSubmitCompetitionDto)
    {
        await _competitionService.UpdateDateSubmitCompetitionAsync(id, updateDateSubmitCompetitionDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update date submit competition successfully!"));
    }
}
