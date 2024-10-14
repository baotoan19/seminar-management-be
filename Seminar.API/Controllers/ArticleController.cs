using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticalByIdAsync(int id)
    {
        ArticleVM article = await _articleService.GetArticleByIdAsync(id);
        return Ok(new BaseResponse<ArticleVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: article));
    }

    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    [HttpPost]
    public async Task<IActionResult> CreateArticalAsync(CreateArticleDto createArticalsDto)
    {
        await _articleService.CreateArticalAsync(createArticalsDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Artical created successfully!"));
    }


    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN + "," + CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateArticalAsync(int id, UpdateArticleDto updateArticalsDto)
    {
        await _articleService.UpdateArticalAsync(id, updateArticalsDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Artical updated successfully!"));
    }

    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticalAsync(int id)
    {
        await _articleService.DeleteArticalAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Artical deleted successfully!"));
    }
}