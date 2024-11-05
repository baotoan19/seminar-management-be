using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/authors")]
[ApiController]
[Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;
    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpPost("create-coauthor")]
    public async Task<IActionResult> CreateCoAuthor(int articleId, CreateCoAuthorDto createCoAuthorDto)
    {
        await _authorService.CreateCoAuthorAsync(articleId, createCoAuthorDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Create co-author success!"));
    }

    [HttpDelete("delete-coauthor")]
    public async Task<IActionResult> DeleteCoAuthor(int articleId, int coAuthorId)
    {
        await _authorService.DeleteCoAuthorAsync(articleId, coAuthorId);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Delete co-author success!"));
    }

    [HttpDelete("delete-member")]
    public async Task<IActionResult> DeleteMember(int researchTopicId, int memberId)
    {
        await _authorService.DeleteMemberAsync(researchTopicId, memberId);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Delete member success!"));
    }
}