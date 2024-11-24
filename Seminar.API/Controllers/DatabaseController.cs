using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.DatabaseDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/authors")]
[ApiController]
[Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
public class DatabaseController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    public DatabaseController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpPost("create-backup")]
    public async Task<IActionResult> CreateBackup(CreateBackupDto createBackupDto)
    {
        await _databaseService.CreateBackupAsync(createBackupDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Tạo backup thành công",
            data: null));
    }

    [HttpPost("restore-backup")]
    public async Task<IActionResult> RestoreBackup([FromForm]CreateRestoreDto createRestoreDto)
    {
        await _databaseService.RestoreBackupAsync(createRestoreDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Khôi phục cơ sở dữ liệu thành công",
            data: null));
    }
}
