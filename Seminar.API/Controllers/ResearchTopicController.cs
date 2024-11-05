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
    public async Task<IActionResult> GetAllResearchTopicByCompetitionId(int competitionId, int index = 1, int pageSize = 8, string nameTopicSearch = "", int disciplineId = 0)
    {
        PaginatedList<ResearchTopicVM> researchTopicVMs = await _researchTopicService.GetAllResearchTopicByCompetitionIdAsync(competitionId, index, pageSize, nameTopicSearch, disciplineId);
        return Ok(new BaseResponse<PaginatedList<ResearchTopicVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topics retrieved successfully",
            data: researchTopicVMs));
    }

    [HttpGet("author")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> GetAllResearchTopicByAuthorId(string roleName = "", int index = 1, int pageSize = 8, string nameTopicSearch = "")
    {
        PaginatedList<ResearchTopicVM> researchTopicVMs = await _researchTopicService.GetAllResearchTopicByAuthorIdAsync(roleName, index, pageSize, nameTopicSearch);
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

    [HttpGet("version-topic/{researchTopicId}")]
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



    [HttpPatch("is-acceptance-approved")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> UpdateIsAcceptanceApproved(UpdateIsAcceptanceApprovedDto updateIsAcceptanceApprovedDto)
    {
        await _researchTopicService.UpdateIsAcceptanceApprovedAsync(updateIsAcceptanceApprovedDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Research topic is acceptance approved updated successfully"));
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
}
