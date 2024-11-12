using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.REVIEWER)]
public class ReviewFormController : ControllerBase
{
    private readonly IReviewFormService _reviewFormService;
    public ReviewFormController(IReviewFormService reviewFormService)
    {
        _reviewFormService = reviewFormService;
    }

    [HttpGet("review-form")]
    [AllowAnonymous]
    [Authorize]
    public async Task<IActionResult> GetAllReviewFormByHistoryUpdateResearchTopicId(int historyUpdateResearchTopicId, int index = 1, int pageSize = 10)
    {
        var result = await _reviewFormService.GetAllReviewFormByHistoryUpdateResearchTopicIdAsync(historyUpdateResearchTopicId, index, pageSize);
        return Ok(new BaseResponse<PaginatedList<ReviewFormVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Form retrieved successfully",
            data: result));
    }

    [HttpPost("review-form")]
    public async Task<IActionResult> CreateReviewForm(CreateReviewFormDto createReviewFormDto)
    {
        await _reviewFormService.CreateReviewFormAsync(createReviewFormDto);
        return Ok(new BaseResponse<CreateReviewFormDto>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Form created successfully",
            data: null));
    }

    [HttpPatch("review-form/{id}")]
    public async Task<IActionResult> UpdateReviewForm(int id, UpdateReviewFormDto updateReviewFormDto)
    {
        await _reviewFormService.UpdateReviewFormAsync(id, updateReviewFormDto);
        return Ok(new BaseResponse<UpdateReviewFormDto>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Form updated successfully",
            data: null));
    }

    [HttpDelete("review-form/{id}")]
    public async Task<IActionResult> DeleteReviewForm(int id)
    {
        await _reviewFormService.DeleteReviewFormAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Review Form deleted successfully",
            data: null));
    }


}
