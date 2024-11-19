using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

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

    [HttpGet("all")]
    public async Task<IActionResult> GetAllPostAsync(int index = 1,int pageSize = 8,string idSearch = "",string nameSearch = "")
    {
        var result = await _postService.GetPagedAsync(index, pageSize, idSearch, nameSearch);
        return Ok(new BaseResponse<PaginatedList<PostVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: result));
    }

    [HttpGet("organizer")]
    [Authorize(Roles =CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> GetPagedByOrganizerIdAsync(int index = 1,int pageSize = 8,string idSearch = "",string nameSearch = "")
    {
        var result = await _postService.GetPagedByOrganizerIdAsync(index, pageSize, idSearch, nameSearch);
        return Ok(new BaseResponse<PaginatedList<PostVM>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Lấy tất cả bài đăng của chủ tịch thành công",
            data: result));
    }

    [HttpPost()]
    [Authorize(Roles =CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> CreatePostAsync(CreatePostDto postDto)
    {
        await _postService.CreatePostAsync(postDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Tạo bài đăng thành công",
            data: null));
    }

    [HttpPatch()]
    [Authorize(Roles =CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> UpdatePostAsync(int id, UpdatePostDto postDto)
    {
        await _postService.UpdatePostAsync(id, postDto);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Cập nhật bài đăng thành công",
            data: null));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles =CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)]
    public async Task<IActionResult> DeletePostAsync(int id)
    {
        await _postService.DeletePostAsync(id);
        return Ok(new BaseResponse<string>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Xóa bài đăng thành công",
            data: null));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostByIdAsync(int id)
    {
        PostVM postVM = await _postService.GetPostByIdAsync(id);
        return Ok(new BaseResponse<PostVM>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: "Lấy bài đăng theo id thành công",
            data: postVM));
    }
    
}