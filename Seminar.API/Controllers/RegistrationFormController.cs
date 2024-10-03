using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegistrationFormController : ControllerBase
{
    private readonly IRegistrationFormService _registrationFormService;

    public RegistrationFormController(IRegistrationFormService registrationFormService)
    {
        _registrationFormService = registrationFormService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewAssignmentByIdAsync(int id)
    {
        RegistrationFormVM registrationFormVM = await _registrationFormService.GetRegistrationFormByIdAsync(id);
        return Ok(new BaseResponse<RegistrationFormVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: registrationFormVM));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRegistrationFormAsync(CURegistrationFormDto cURegistrationFormDto)
    {
        await _registrationFormService.CreateRegistrationFormAsync(cURegistrationFormDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Create registration form successfully!"));
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateRegistrationFormAsync(int id, CURegistrationFormDto cURegistrationFormDto)
    {
        await _registrationFormService.UpdateRegistrationFormAsync(id, cURegistrationFormDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Update registration form successfully!"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReviewAssignmentAsync(int id)
    {
        await _registrationFormService.DeleteRegistrationFormAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Delete registration form successfully!"));
    }
}
