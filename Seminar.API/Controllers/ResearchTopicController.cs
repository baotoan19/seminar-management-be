using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResearchTopicController : ControllerBase
{
    private readonly IResearchTopicService _researchTopicService;
    public ResearchTopicController(IResearchTopicService researchTopicService)
    {
        _researchTopicService = researchTopicService;
    }

    [HttpGet("competition/{competitionId}")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> GetAllResearchTopicByCompetitionId(int competitionId, int reviewCommitteeId, int index = 1, int pageSize = 8, string nameTopicSearch = "", int disciplineId = 0, int acceptedForPublicationStatus = 3, int ReviewAcceptanceStatus = 3)
    {
        PaginatedList<ResearchTopicVM> researchTopicVMs = await _researchTopicService.GetAllResearchTopicByCompetitionIdAsync(competitionId, reviewCommitteeId, index, pageSize, nameTopicSearch, disciplineId, acceptedForPublicationStatus, ReviewAcceptanceStatus);
        return Ok(new BaseResponse<PaginatedList<ResearchTopicVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topics retrieved successfully",
            data: researchTopicVMs));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResearchTopicById(int id)
    {
        ResearchTopicVM researchTopicVM = await _researchTopicService.GetResearchTopicByIdAsync(id);
        return Ok(new BaseResponse<ResearchTopicVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topic retrieved successfully",
            data: researchTopicVM));
    }

    [HttpGet("author")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> GetAllResearchTopicByAuthorId(string roleName = "", int index = 1, int pageSize = 8, string nameTopicSearch = "", int acceptedForPublicationStatus = 3, int ReviewAcceptanceStatus = 3, int competitionId = 0)
    {
        PaginatedList<ResearchTopicVM> researchTopicVMs = await _researchTopicService.GetAllResearchTopicByAuthorIdAsync(roleName, index, pageSize, nameTopicSearch, acceptedForPublicationStatus, ReviewAcceptanceStatus, competitionId);
        return Ok(new BaseResponse<PaginatedList<ResearchTopicVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topics retrieved successfully",
            data: researchTopicVMs));
    }

    [HttpGet("review")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.REVIEWER)]
    public async Task<IActionResult> GetResearchTopicsForReview(int index = 1, int pageSize = 8, int idSearch = 0, string nameTopicSearch = "", int isStatus = 0, int competitionId = 0)
    {
        PaginatedList<ResearchTopicVM> researchTopicVMs = await _researchTopicService.GetResearchTopicsForReviewAsync(index, pageSize, idSearch, nameTopicSearch, isStatus, competitionId);
        return Ok(new BaseResponse<PaginatedList<ResearchTopicVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topics retrieved successfully",
            data: researchTopicVMs));
    }

    [HttpGet("version-topic/{researchTopicId}")]
    [Authorize]
    public async Task<IActionResult> GetAllHistoryResearchTopicByResearchTopicId(int researchTopicId)
    {
        List<HistoryUpdateResearchTopicVM> historyUpdateResearchTopicVMs = await _researchTopicService.GetAllHistoryResearchTopicByResearchTopicIdAsync(researchTopicId);
        return Ok(new BaseResponse<List<HistoryUpdateResearchTopicVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "History research topic retrieved successfully",
            data: historyUpdateResearchTopicVMs));
    }

    [HttpPost]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> CreateResearchTopic(CreateResearchTopicDto createResearchTopicDto)
    {
        await _researchTopicService.CreateResearchTopicAsync(createResearchTopicDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topic created successfully"));
    }

    [HttpPost("new-version")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> CreateNewVersionResearchTopic(CreateHistoryResearchTopicDto createHistoryResearchTopicDto)
    {
        await _researchTopicService.CreateNewVersionResearchTopicAsync(createHistoryResearchTopicDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "New version research topic created successfully"));
    }

    [HttpPatch("update/{researchTopicId}")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> UpdateResearchTopic(int researchTopicId, UpdateResearchTopicDto updateResearchTopicDto)
    {
        await _researchTopicService.UpdateResearchTopicAsync(researchTopicId, updateResearchTopicDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topic updated successfully"));
    }

    [HttpPatch("is-review-acceptance")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> UpdateIsReviewAcceptance(UpdateIsReviewAcceptanceDto updateIsReviewAcceptanceDto)
    {
        await _researchTopicService.UpdateIsReviewAcceptanceAsync(updateIsReviewAcceptanceDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topic is review acceptance updated successfully"));
    }

    [HttpPatch("date-end/{researchTopicId}")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> UpdateDateEndResearchTopic(int researchTopicId, UpdateDateEndResearchTopicDto updateDateEndResearchTopicDto)
    {
        await _researchTopicService.UpdateDateEndResearchTopicAsync(researchTopicId, updateDateEndResearchTopicDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topic date end updated successfully"));
    }

    [HttpPatch("history-research-topic/{historyResearchTopicId}")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> UpdateHistoryResearchTopic(int historyResearchTopicId, UpdateHistoryResearchTopicDto updateHistoryResearchTopicDto)
    {
        await _researchTopicService.UpdateHistoryResearchTopicAsync(historyResearchTopicId, updateHistoryResearchTopicDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "History research topic updated successfully"));
    }

    [HttpDelete("history-research-topic/{historyResearchTopicId}")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> DeleteHistoryResearchTopic(int historyResearchTopicId)
    {
        await _researchTopicService.DeleteHistoryResearchTopicAsync(historyResearchTopicId);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "History research topic deleted successfully"));
    }
}
