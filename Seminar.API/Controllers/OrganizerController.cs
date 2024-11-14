using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Interfaces;
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
    private readonly IReviewFormService _reviewFormService;
    public OrganizerController(IOrganizerService organizerService, IReviewFormService reviewFormService)
    {
        _organizerService = organizerService;
        _reviewFormService = reviewFormService;
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

    [HttpGet("review-committee/research-topic/{researchTopicId}")]
    [AllowAnonymous]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER},{CLAIMS_VALUES.ROLE_TYPE.AUTHOR}")]
    public async Task<IActionResult> GetReviewCommitteeByResearchTopicIdAsync(int researchTopicId, int page = 1, int pageSize = 10, int idSearch = 0, string nameSearch = "")
    {
        var result = await _organizerService.GetReviewCommitteeByResearchTopicIdAsync(researchTopicId, page, pageSize, idSearch, nameSearch);
        return Ok(new BaseResponse<PaginatedList<ReviewCommitteeVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Committee retrieved successfully",
            data: result));
    }

    [HttpGet("review-committee/{id}")]
    public async Task<IActionResult> GetReviewCommitteeByIdAsync(int id)
    {
        var result = await _organizerService.GetReviewCommitteeByIdAsync(id);
        return Ok(new BaseResponse<ReviewCommitteeVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Committee retrieved successfully",
            data: result));
    }

    [HttpPatch("review-committee/{id}")]
    public async Task<IActionResult> UpdateReviewCommittee(int id, UpdateReviewCommitteeDto updateReviewCommitteeDto)
    {
        await _organizerService.UpdateReviewCommitteeAsync(id, updateReviewCommitteeDto);
        return Ok(new BaseResponse<UpdateReviewCommitteeDto>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Committee updated successfully",
            data: null));
    }

    [HttpPatch("review-committee/assign/{researchTopicId}")]
    public async Task<IActionResult> AssignReviewCommitteeToResearchTopic(int researchTopicId, UpdateReviewCommitteeIdDto updateReviewCommitteeIdDto)
    {
        await _organizerService.AssignReviewCommitteeToResearchTopicAsync(researchTopicId, updateReviewCommitteeIdDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Committee assigned to research topic successfully",
            data: null));
    }

    [HttpPatch("review-committee/date-end/{id}")]
    public async Task<IActionResult> UpdateDateEndReviewCommittee(int id, UpdateDateEndReviewCommitteeDto updateDateEndReviewCommitteeDto)
    {
        await _organizerService.UpdateDateEndReviewCommitteeAsync(id, updateDateEndReviewCommitteeDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Date end review committee updated successfully",
            data: null));
    }

    [HttpDelete("review-committee/{id}")]
    public async Task<IActionResult> DeleteReviewCommittee(int id)
    {
        await _organizerService.DeleteReviewCommitteeAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Committee deleted successfully",
            data: null));
    }
}