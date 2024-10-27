using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

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
    
}