using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.PostDto;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Common;
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

    public async Task<PaginatedList<PostVM>> GetPagedAsync(int index, int pageSize, string idSearch, string nameSearch)
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }

        IQueryable<Post> query = _unitOfWork.GetRepository<Post>().Entities
            .Include(p => p.Organizers)
                .Where(p => p.DeletedAt == null)
                    .OrderByDescending(p => p.CreatedAt);

        //Tìm kiếm theo id
        if (!string.IsNullOrEmpty(idSearch))
        {
            query = query.Where(p => p.Id.ToString().Contains(idSearch));
            bool isInt = int.TryParse(idSearch, out int idInt);
            if (!isInt)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Post not found!");
            }
        }


        //Tìm kiếm theo tên
        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Post not found!");
            }
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<PostVM>(new List<PostVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index -1 )* pageSize).Take(pageSize).ToListAsync();
        List<PostVM> responeItems = _mapper.Map<List<PostVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<PostVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }

    public async Task CreatePostAsync(CreatePostDto postDto)
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
        post.IsStatus = true;
        await _unitOfWork.GetRepository<Post>().InsertAsync(post);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(int id, UpdatePostDto postDto)
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