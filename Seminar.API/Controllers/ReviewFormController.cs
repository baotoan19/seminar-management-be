using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewFormController : ControllerBase
{
    private readonly IReviewFormService _reviewFormService;

    public ReviewFormController(IReviewFormService reviewFormService)
    {
        _reviewFormService = reviewFormService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviewFormById(int id)
    {
        ReviewFormVM reviewFormVM = await _reviewFormService.GetReviewFormByIdAsync(id);
        return Ok(new BaseResponse<ReviewFormVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: reviewFormVM));
    }

    [HttpPost]
    public async Task<IActionResult> CreateReviewForm(CUReviewFormDto cUReviewFormDto)
    {
        await _reviewFormService.CreateReviewFormAsync(cUReviewFormDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Create review form successfully!"));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateReviewForm(int id, CUReviewFormDto cUReviewFormDto)
    {
        await _reviewFormService.UpdateReviewFormAsync(id, cUReviewFormDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Update review form successfully!"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReviewForm(int id)
    {
        await _reviewFormService.DeleteReviewFormAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Delete review form successfully!"));
    }
}
