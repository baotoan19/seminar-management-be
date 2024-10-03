using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewCommitteeController : ControllerBase
{
    private readonly IReviewCommitteeService _reviewCommitteeService;

    public ReviewCommitteeController(IReviewCommitteeService reviewCommitteeService)
    {
        _reviewCommitteeService = reviewCommitteeService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewCommitteeByIdAsync(int id)
    {
        ReviewCommitteeVM reviewCommitteeVM = await _reviewCommitteeService.GetReviewCommitteeByIdAsync(id);
        return Ok(new BaseResponse<ReviewCommitteeVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: reviewCommitteeVM));
    }

    [HttpPost]
    public async Task<IActionResult> CreateReviewCommitteeAsync(CUReviewCommitteeDto cUReviewCommitteeDto)
    {
        await _reviewCommitteeService.CreateReviewCommitteeAsync(cUReviewCommitteeDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Create review committee successfully!"));
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateReviewCommitteeAsync(int id, CUReviewCommitteeDto cUReviewCommitteeDto)
    {
        await _reviewCommitteeService.UpdateReviewCommitteeAsync(id, cUReviewCommitteeDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Update review committee successfully!"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReviewAssignmentAsync(int id)
    {
        await _reviewCommitteeService.DeleteReviewCommitteeAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Delete review committee successfully!"));
    }
}
