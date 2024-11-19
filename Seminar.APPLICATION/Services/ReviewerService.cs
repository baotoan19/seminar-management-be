using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class ReviewerService : IReviewerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewerService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReviewerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewerService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // public async Task<Reviewer> CreateReviewerAsync(CreateReviewerDto createReviewerDto)
    // {
    //     Reviewer? existsReviewer = await _unitOfWork.GetRepository<Reviewer>().Entities.FirstOrDefaultAsync(r => r.AccountId == createReviewerDto.AccountId);
    //     if (existsReviewer != null)
    //     {
    //         throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Reviewer is existed!");
    //     }
    //     Reviewer reviewer = _mapper.Map<Reviewer>(createReviewerDto);
    //     await _unitOfWork.GetRepository<Reviewer>().InsertAsync(reviewer);
    //     await _unitOfWork.SaveChangesAsync();
    //     return reviewer;
    // }

    public async Task<ReviewerVM> GetReviewerInforAsync(int id)
    {
        Reviewer? reviewer = await _unitOfWork.GetRepository<Reviewer>().Entities.Include(r => r.Account).Include(r => r.Faculty).FirstOrDefaultAsync(r => r.AccountId == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người đánh giá không tồn tại. Vui lòng cung cấp người đánh giá hợp lệ.");
        ReviewerVM reviewerVM = new ReviewerVM
        {
            Id = reviewer.Id,
            Name = reviewer.Name ?? "Unknown",
            Email = reviewer.Account.Email ?? "Unknown",
            FacultyId = reviewer.FacultyId ?? 0,
            FacultyName = reviewer.Faculty?.FacultyName ?? "Unknown",
            AccountId = reviewer.AccountId,
            DateOfBirth = reviewer.DateOfBirth ?? DateTime.MinValue,
            Sex = reviewer.Sex ?? "Unknown",
            NumberPhone = reviewer.NumberPhone ?? "Unknown",
            AcademicDegree = reviewer.AcademicDegree ?? "Unknown",
            AcademicRank = reviewer.AcademicRank ?? "Unknown"
        };
        return reviewerVM;
    }

    public async Task UpdateReviewerAsync(int id, UpdateReviewerDto updateReviewerDto)
    {
        Reviewer? reviewer = await _unitOfWork.GetRepository<Reviewer>().Entities.Include(r => r.Account).Include(r => r.Faculty).FirstOrDefaultAsync(r => r.AccountId == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người đánh giá không tồn tại. Vui lòng cung cấp người đánh giá hợp lệ.");
        _mapper.Map(updateReviewerDto, reviewer);
        reviewer.Account.Email = updateReviewerDto.Email;
        reviewer.Account.UpdatedAt = DateTime.Now;
        reviewer.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Reviewer>().UpdateAsync(reviewer);
        await _unitOfWork.SaveChangesAsync();
    }

}
