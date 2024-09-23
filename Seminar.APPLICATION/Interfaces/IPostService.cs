using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Models;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces;

public interface IPostService
{
    Task CreatePostAsync(PostDto postDto);
    Task UpdatePostAsync(int id, PostDto postDto);
    Task DeletePostAsync(int id);
    Task<PostVM> GetPostByIdAsync(int id);
}