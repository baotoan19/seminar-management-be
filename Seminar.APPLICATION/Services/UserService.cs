using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _accountService;
    private readonly IAuthorService _authorService;
    private readonly IReviewerService _reviewerService;
    private readonly IOrganizerService _organizerService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IAccountService accountService, IAuthorService authorService, IReviewerService reviewerService, IOrganizerService organizerService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _accountService = accountService;
        _authorService = authorService;
        _reviewerService = reviewerService;
        _organizerService = organizerService;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserVM> GetUserInforAsync()
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        if (!int.TryParse(userId, out int accountId))
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_USER_ID, "Invalid user id!");
        }

        Account? account = await _unitOfWork.GetRepository<Account>().Entities.Include(a => a.Role).FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Account not found!");
        }
        switch (account.Role.RoleName)
        {
            case CLAIMS_VALUES.ROLE_TYPE.AUTHOR:
                AuthorVM authorVM = await _authorService.GetAuthorInforAsync(accountId);
                return authorVM;
            case CLAIMS_VALUES.ROLE_TYPE.REVIEWER:
                ReviewerVM reviewerVM = await _reviewerService.GetReviewerInforAsync(accountId);
                return reviewerVM;
            case CLAIMS_VALUES.ROLE_TYPE.ORGANIZER:
                OrganizerVM organizerVM = await _organizerService.GetOrganizerInforAsync(accountId);
                return organizerVM;
            default:
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_ROLE, "Invalid role!");
        }
    }

    public async Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        if (!int.TryParse(userId, out int accountId))
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_USER_ID, "Invalid user id!");
        }
        await _authorService.UpdateAuthorAsync(accountId, updateAuthorDto);
    }

    public async Task UpdateReviewerAsync(UpdateReviewerDto updateReviewerDto)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        if (!int.TryParse(userId, out int accountId))
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_USER_ID, "Invalid user id!");
        }
        await _reviewerService.UpdateReviewerAsync(accountId, updateReviewerDto);
    }

    public async Task UpdateOrganizerAsync(UpdateOrganizerDto updateOrganizerDto)
    {
        string userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        if (!int.TryParse(userId, out int accountId))
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_USER_ID, "Invalid user id!");
        }
        await _organizerService.UpdateOrganizerAsync(accountId, updateOrganizerDto);
    }
}