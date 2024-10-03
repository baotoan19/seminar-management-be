using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FacultyController : ControllerBase
{
    private readonly IFacultyService _facultyService;

    public FacultyController(IFacultyService facultyService)
    {
        _facultyService = facultyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFacultyAsync()
    {
        IList<FacultyVM> faculties = await _facultyService.GetAllFacultyAsync();
        return Ok(new BaseResponse<IList<FacultyVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: faculties));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFacultyByIdAsync(int id)
    {
        FacultyVM faculty = await _facultyService.GetFacultyByIdAsync(id);
        return Ok(new BaseResponse<FacultyVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: faculty));
    }
}
        