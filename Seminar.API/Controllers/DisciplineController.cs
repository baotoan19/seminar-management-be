using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DisciplineController : ControllerBase
{
    private readonly IDisciplineService _disciplineService;

    public DisciplineController(IDisciplineService disciplineService)
    {
        _disciplineService = disciplineService;
    }
    [HttpGet()]
    public async Task<IActionResult> GetAllAsync()
    {
        IList<DisciplineVM> disciplines = await _disciplineService.GetAllDisciplineAsync();
        return Ok(new BaseResponse<IList<DisciplineVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: disciplines));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        DisciplineVM discipline = await _disciplineService.GetDisciplineByIdAsync(id);
        return Ok(new BaseResponse<DisciplineVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: discipline));
    }
}