using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class ReviewAssignmentService : IReviewAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewAssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReviewAssignmentVM> GetReviewAssignmentByIdAsync(int id)
    {
        Review_Assignment review_Assignment = await _unitOfWork.GetRepository<Review_Assignment>().Entities.Include(x => x.Artical)
            .Include(x => x.Reviewer).Include(x => x.Organizer).FirstOrDefaultAsync(x => x.Id == id) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Assignment not found!");
        ReviewAssignmentVM reviewAssignmentVM = _mapper.Map<ReviewAssignmentVM>(review_Assignment);
        reviewAssignmentVM.ArticalTitle = review_Assignment.Artical.Title;
        reviewAssignmentVM.ReviewerName = review_Assignment.Reviewer.Name;
        reviewAssignmentVM.OrganizerName = review_Assignment.Organizer.Name;
        return reviewAssignmentVM;
    }

    public async Task CreateReviewAssignmentAsync(CUReviewAssignmentDto dto)
    {
        Review_Assignment reviewAssignment = _mapper.Map<Review_Assignment>(dto);
        Artical artical = await _unitOfWork.GetRepository<Artical>().GetByIdAsync(dto.ArticalId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        Reviewer reviewer = await _unitOfWork.GetRepository<Reviewer>().GetByIdAsync(dto.ReviewerId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Reviewer not found!");
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().GetByIdAsync(dto.OrganizerId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        reviewAssignment.Status  = true;
        await _unitOfWork.GetRepository<Review_Assignment>().InsertAsync(reviewAssignment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateReviewAssignmentAsync(int id, CUReviewAssignmentDto dto)
    {
        Review_Assignment reviewAssignment = await _unitOfWork.GetRepository<Review_Assignment>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Assignment not found!");
        Artical artical = await _unitOfWork.GetRepository<Artical>().GetByIdAsync(dto.ArticalId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        Reviewer reviewer = await _unitOfWork.GetRepository<Reviewer>().GetByIdAsync(dto.ReviewerId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Reviewer not found!");
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().GetByIdAsync(dto.OrganizerId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        _mapper.Map(dto, reviewAssignment);
        reviewAssignment.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Assignment>().UpdateAsync(reviewAssignment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteReviewAssignmentAsync(int id)
    {
        Review_Assignment reviewAssignment = await _unitOfWork.GetRepository<Review_Assignment>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Assignment not found!");
        reviewAssignment.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Assignment>().UpdateAsync(reviewAssignment);
        await _unitOfWork.SaveChangesAsync();
    }
}