using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Models;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Interfaces;

public interface IPostService
{
    Task<PaginatedList<PostVM>> GetPagedAsync(int index, int pageSize,string idSearch,string nameSearch);
    Task CreatePostAsync(CreatePostDto postDto);
    Task UpdatePostAsync(int id, UpdatePostDto postDto);
    Task DeletePostAsync(int id);
    Task<PostVM> GetPostByIdAsync(int id);
}