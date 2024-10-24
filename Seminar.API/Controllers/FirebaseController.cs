using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.FirebaseDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FirebaseController : ControllerBase
{
    private readonly IFirebaseService _firebaseService;
    public FirebaseController(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFileAsync([FromForm] CreateFirebaseDto createFirebaseDto)
    {
        var result = await _firebaseService.UploadFileAsync(createFirebaseDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Upload file successfully",
            data: result));
    }

    [HttpDelete("delete-file")]
    public async Task<IActionResult> DeleteFileAsync(string fileName)
    {
        await _firebaseService.DeleteFileAsync(fileName);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Delete file successfully"));
    }
}
