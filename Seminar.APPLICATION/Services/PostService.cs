using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task CreatePostAsync(PostDto postDto)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        if (!int.TryParse(userId, out int userIdInt))
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid user ID");
        }
        Organizer? organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userIdInt) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        Post post = _mapper.Map<Post>(postDto);
        post.OrganizerId = organizer.Id;
        await _unitOfWork.GetRepository<Post>().InsertAsync(post);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(int id, PostDto postDto)
    {
        Post? post = await _unitOfWork.GetRepository<Post>().Entities.FirstOrDefaultAsync(p => p.Id == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Post not found!");
        _mapper.Map(postDto, post);
        post.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Post>().UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int id)
    {
        Post? post = await _unitOfWork.GetRepository<Post>().Entities.FirstOrDefaultAsync(p => p.Id == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Post not found!");
        post.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Post>().UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PostVM> GetPostByIdAsync(int id)
    {
        Post? post = await _unitOfWork.GetRepository<Post>().Entities.Include(p => p.Organizers).FirstOrDefaultAsync(p => p.Id == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Post not found!");
        PostVM postVM = _mapper.Map<PostVM>(post);
        postVM.OrganizerName = post.Organizers.Name;
        return postVM;
    }
}