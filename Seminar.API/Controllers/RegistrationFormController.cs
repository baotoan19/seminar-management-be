using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

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

    [HttpGet]
    public async Task<IActionResult> GetPagedAsync(int index = 1, int pageSize = 8, string idSearch = "", string nameSearch = "")
    {
        PaginatedList<RegistrationFormVM> registrationFormVMs = await _registrationFormService.GetPagedAsync(index, pageSize, idSearch, nameSearch);
        return Ok(new BaseResponse<PaginatedList<RegistrationFormVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: registrationFormVMs));
    }

    [HttpGet("author")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.AUTHOR}")]
    public async Task<IActionResult> GetAllByAuthorIdAsync()
    {
        List<RegistrationFormVM> registrationFormVMs = await _registrationFormService.GetAllByAuthorIdAsync();
        return Ok(new BaseResponse<List<RegistrationFormVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: registrationFormVMs));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRegistrationFormByIdAsync(int id)
    {
        RegistrationFormVM registrationFormVM = await _registrationFormService.GetRegistrationFormByIdAsync(id);
        return Ok(new BaseResponse<RegistrationFormVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: registrationFormVM));
    }

    [HttpPost]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.AUTHOR}")]
    public async Task<IActionResult> CreateRegistrationFormAsync(CreateRegistrationFormDto createRegistrationFormDto)
    {
        await _registrationFormService.CreateRegistrationFormAsync(createRegistrationFormDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Create registration form successfully!"));
    }
    [HttpPatch("{id}")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}")]
    public async Task<IActionResult> UpdateRegistrationFormAsync(int id, UpdateRegistrationFormDto updateRegistrationFormDto)
    {
        await _registrationFormService.UpdateRegistrationFormAsync(id, updateRegistrationFormDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Update registration form successfully!"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}")]
    public async Task<IActionResult> DeleteRegistrationFormAsync(int id)
    {
        await _registrationFormService.DeleteRegistrationFormAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Delete registration form successfully!"));
    }
}
