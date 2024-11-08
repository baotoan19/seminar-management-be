using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

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


}