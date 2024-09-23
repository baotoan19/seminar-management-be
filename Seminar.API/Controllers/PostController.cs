using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.DOMAIN.Entitys;

namespace Seminar.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost("create-post")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}")]
    public async Task<IActionResult> CreatePostAsync(PostDto postDto)
    {
        await _postService.CreatePostAsync(postDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Create post successfully!"));
    }

    [HttpPatch("update-post/{id}")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}")]
    public async Task<IActionResult> UpdatePostAsync(int id, PostDto postDto)
    {
        await _postService.UpdatePostAsync(id, postDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Update post successfully!"));
    }

    [HttpDelete("delete-post/{id}")]
    [Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}")]
    public async Task<IActionResult> DeletePostAsync(int id)
    {
        await _postService.DeletePostAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: "Delete post successfully!"));
    }

    [HttpGet("get-post/{id}")]
    //[Authorize(Roles = $"{CLAIMS_VALUES.ROLE_TYPE.ORGANIZER}")]
    public async Task<IActionResult> GetPostByIdAsync(int id)
    {
        PostVM postVM = await _postService.GetPostByIdAsync(id);
        return Ok(new BaseResponse<PostVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: postVM));
    }
    
}