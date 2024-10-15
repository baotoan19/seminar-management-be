using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("infor")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}, {CLAIMS_VALUES.ROLE_TYPE.AUTHOR}, {CLAIMS_VALUES.ROLE_TYPE.REVIEWER}")]
    public async Task<IActionResult> GetUserInfor()
    {
        UserVM userVM = await _userService.GetUserInforAsync();

        if (userVM is OrganizerVM organizerVM)
        {
            return Ok(new BaseResponse<OrganizerVM>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: organizerVM));
        }
        else if (userVM is AuthorVM authorVM)
        {
            return Ok(new BaseResponse<AuthorVM>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: authorVM));
        }
        else if (userVM is ReviewerVM reviewerVM)
        {
            return Ok(new BaseResponse<ReviewerVM>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: reviewerVM));
        }
        return BadRequest(new BaseResponse<UserVM>(
            statusCode: StatusCodes.Status400BadRequest,
            code: ResponseCodeConstants.INVALID_ROLE,
            data: null));
    }

    [HttpPatch("update-author")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.AUTHOR}")]
    public async Task<IActionResult> UpdateAuthor(UpdateAuthorDto updateAuthorDto)
    {
        await _userService.UpdateAuthorAsync(updateAuthorDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update author success"));
    }

    [HttpPatch("update-reviewer")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.REVIEWER}")]
    public async Task<IActionResult> UpdateReviewer(UpdateReviewerDto updateReviewerDto)
    {
        await _userService.UpdateReviewerAsync(updateReviewerDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update reviewer success"));
    }

    [HttpPatch("update-organizer")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}")]
    public async Task<IActionResult> UpdateOrganizer(UpdateOrganizerDto updateOrganizerDto)
    {
        await _userService.UpdateOrganizerAsync(updateOrganizerDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update organizer success"));
    }

}