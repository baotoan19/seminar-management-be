// using Microsoft.AspNetCore.Mvc;
// using Seminar.APPLICATION.Dtos.ArticalsDtos;
// using Seminar.APPLICATION.Interfaces;
// using Seminar.APPLICATION.Models;
// using Seminar.CORE.Base;
// using Seminar.CORE.Constants;

// namespace Seminar.API.Controllers;

// [Route("api/[controller]")]
// [ApiController]
// public class ArticalController : ControllerBase
// {
//     private readonly IArticalsService _articalService;

//     public ArticalController(IArticalsService articalService)
//     {
//         _articalService = articalService;
//     }

//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetArticalByIdAsync(int id)
//     {
//         ArticalVM artical = await _articalService.GetArticalByIdAsync(id);
//         return Ok(new BaseResponse<ArticalVM>(
//             statusCode: StatusCodes.Status200OK,
//             code: ResponseCodeConstants.SUCCESS,
//             data: artical));
//     }

//     [HttpPost]
//     public async Task<IActionResult> CreateArticalAsync(CUArticalsDto createArticalsDto)
//     {
//         await _articalService.CreateArticalAsync(createArticalsDto);
//         return Ok(new BaseResponse<string>(
//             statusCode: StatusCodes.Status200OK,
//             code: ResponseCodeConstants.SUCCESS,
//             message: "Artical created successfully!"));
//     }

//     [HttpPatch("{id}")]
//     public async Task<IActionResult> UpdateArticalAsync(int id, CUArticalsDto updateArticalsDto)
//     {
//         await _articalService.UpdateArticalAsync(id, updateArticalsDto);
//         return Ok(new BaseResponse<string>(
//             statusCode: StatusCodes.Status200OK,
//             code: ResponseCodeConstants.SUCCESS,
//             message: "Artical updated successfully!"));
//     }

//     [HttpDelete("{id}")]
//     public async Task<IActionResult> DeleteArticalAsync(int id)
//     {
//         await _articalService.DeleteArticalAsync(id);
//         return Ok(new BaseResponse<string>(
//             statusCode: StatusCodes.Status200OK,
//             code: ResponseCodeConstants.SUCCESS,
//             message: "Artical deleted successfully!"));
//     }
// }