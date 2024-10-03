using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewAssignmentController : ControllerBase
{
    private readonly IReviewAssignmentService _reviewAssignmentService;

    public ReviewAssignmentController(IReviewAssignmentService reviewAssignmentService)
    {
        _reviewAssignmentService = reviewAssignmentService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewAssignmentByIdAsync(int id)
    {
        ReviewAssignmentVM reviewAssignmentVM = await _reviewAssignmentService.GetReviewAssignmentByIdAsync(id);
        return Ok(new BaseResponse<ReviewAssignmentVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: reviewAssignmentVM));
    }

    [HttpPost]
    public async Task<IActionResult> CreateReviewAssignmentAsync(CUReviewAssignmentDto cUReviewAssignmentDto)
    {
        await _reviewAssignmentService.CreateReviewAssignmentAsync(cUReviewAssignmentDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Create review assignment successfully!"));
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateReviewAssignmentAsync(int id, CUReviewAssignmentDto cUReviewAssignmentDto)
    {
        await _reviewAssignmentService.UpdateReviewAssignmentAsync(id, cUReviewAssignmentDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Update review assignment successfully!"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReviewAssignmentAsync(int id)
    {
        await _reviewAssignmentService.DeleteReviewAssignmentAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Delete review assignment successfully!"));
    }
}
