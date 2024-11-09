using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
public class OrganizerController : ControllerBase
{
    private readonly IOrganizerService _organizerService;
    public OrganizerController(IOrganizerService organizerService)
    {
        _organizerService = organizerService;
    }

    //Review Committee
    [HttpPost("review-committee")]
    public async Task<IActionResult> CreateReviewCommittee([FromBody] CreateReviewCommitteeDto createReviewCommitteeDto)
    {
        await _organizerService.CreateReviewCommitteeAsync(createReviewCommitteeDto);
        return Ok(new BaseResponse<List<CreateReviewCommitteeDto>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Committee created successfully",
            data: null));
    }

    [HttpGet("review-committee")]
    public async Task<IActionResult> GetReviewCommitteeByCompetitionIdAsync(int competitionId, int page = 1, int pageSize = 10, int idSearch = 0, string nameSearch = "")
    {
        var result = await _organizerService.GetReviewCommitteeByCompetitionIdAsync(competitionId, page, pageSize, idSearch, nameSearch);
        return Ok(new BaseResponse<PaginatedList<ReviewCommitteeVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Committee retrieved successfully",
            data: result));
    }

    //Review Assignment
    [HttpPost("review-assignment")]
    public async Task<IActionResult> CreateReviewAssignment([FromBody] CreateReviewAssignmentDto createReviewAssignmentDto)
    {
        await _organizerService.CreateReviewAssignmentAsync(createReviewAssignmentDto);
        return Ok(new BaseResponse<List<CreateReviewAssignmentDto>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Assignment created successfully",
            data: null));
    }


}