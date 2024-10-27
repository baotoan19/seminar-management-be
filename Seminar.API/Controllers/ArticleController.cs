using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

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

    [HttpGet("paging-admin")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
    public async Task<IActionResult> GetAllArticlesPagedAsync(int index = 1, int pageSize = 8, string idSearch = "", string nameSearch = "")
    {
        PaginatedList<ArticleVM> articles = await _articleService.GetAllArticlesPagedAsync(index, pageSize, idSearch, nameSearch);
        return Ok(new BaseResponse<PaginatedList<ArticleVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: articles));
    }

    [HttpGet("paging-user")]
    public async Task<IActionResult> GetApprovedArticlesPagedAsync(int index = 1, int pageSize = 8, string idSearch = "", string nameSearch = "")
    {
        PaginatedList<ArticleVM> articles = await _articleService.GetApprovedArticlesPagedAsync(index, pageSize, idSearch, nameSearch);
        return Ok(new BaseResponse<PaginatedList<ArticleVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: articles));
    }

    [HttpGet()]
    public async Task<IActionResult> GetArticleByIdAsync(int id)
    {
        ArticleVM article = await _articleService.GetArticleByIdAsync(id);
        return Ok(new BaseResponse<ArticleVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: article));
    }

    [HttpPost]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    public async Task<IActionResult> CreateArticleAsync(CreateArticleDto createArticleDto)
    {
        await _articleService.CreateArticleAsync(createArticleDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Article created successfully!"));
    }


    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
    [HttpPatch()]
    public async Task<IActionResult> UpdateArticleAsync(int ArticleId, UpdateArticleDto updateArticleDto)
    {
        await _articleService.UpdateArticleAsync(ArticleId, updateArticleDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Article updated successfully!"));
    }

    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
    [HttpDelete()]
    public async Task<IActionResult> DeleteArticleAsync(int id)
    {
        await _articleService.DeleteArticleAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Article deleted successfully!"));
    }

    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    [HttpGet("role")]
    public async Task<IActionResult> GetAuthorArticleByRoleAsync(string roleName)
    {
        List<AuthorArticleVM> authorArticleVMs = await _articleService.GetAuthorArticleByRoleAsync(roleName);
        return Ok(new BaseResponse<List<AuthorArticleVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: authorArticleVMs));
    }


    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.AUTHOR)]
    [HttpGet("author")]
    public async Task<IActionResult> GetAuthorArticleByAuthorIdAsync()
    {
        List<AuthorArticleVM> authorArticleVMs = await _articleService.GetAuthorArticleByAuthorIdAsync();
        return Ok(new BaseResponse<List<AuthorArticleVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: authorArticleVMs));
    }

    [HttpGet("article-id-authors")]
    public async Task<IActionResult> GetAuthorByArticleIdAsync(int articleId)
    {
        List<ArticleAuthorVM> authorArticleVMs = await _articleService.GetAuthorByArticleIdAsync(articleId);
        return Ok(new BaseResponse<List<ArticleAuthorVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: authorArticleVMs));
    }

    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
    [HttpPatch("approve")]
    public async Task<IActionResult> ApproveArticleAsync(int id, ApproveArticleDto approveArticleDto)
    {
        await _articleService.ApproveArticleAsync(id, approveArticleDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Article approved successfully!"));
    }
}