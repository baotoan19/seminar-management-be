using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class ReviewFormService : IReviewFormService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewFormService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReviewFormVM> GetReviewFormByIdAsync(int id)
    {
        Review_Form reviewForm = await _unitOfWork.GetRepository<Review_Form>().Entities.Include(r => r.Conclude).Include(r => r.Reviewer).FirstOrDefaultAsync(r => r.Id == id) ?? 
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Form not found!");
        ReviewFormVM reviewFormVM = _mapper.Map<ReviewFormVM>(reviewForm);
        reviewFormVM.Conclude = _mapper.Map<ConcludeVM>(reviewForm.Conclude);
        reviewFormVM.Reviewer = _mapper.Map<ReviewerVM>(reviewForm.Reviewer);
        return reviewFormVM;
    }

    public async Task CreateReviewFormAsync(CUReviewFormDto createReviewFormDto)
    {
        Review_Form reviewForm = _mapper.Map<Review_Form>(createReviewFormDto);
        History_Update_Artical historyUpdateArtical = await _unitOfWork.GetRepository<History_Update_Artical>().GetByIdAsync(createReviewFormDto.HistoryId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "History Update Artical not found!");
        Reviewer reviewer = await _unitOfWork.GetRepository<Reviewer>().GetByIdAsync(createReviewFormDto.ReviewerId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Reviewer not found!");
        Conclude conclude = await _unitOfWork.GetRepository<Conclude>().GetByIdAsync(createReviewFormDto.ConcludeId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Conclude not found!");
        reviewForm.CreatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Form>().InsertAsync(reviewForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateReviewFormAsync(int id, CUReviewFormDto updateReviewFormDto)
    {
        Review_Form reviewForm = await _unitOfWork.GetRepository<Review_Form>().GetByIdAsync(id) ?? 
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Form not found!");
        History_Update_Artical historyUpdateArtical = await _unitOfWork.GetRepository<History_Update_Artical>().GetByIdAsync(updateReviewFormDto.HistoryId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "History Update Artical not found!");
        Reviewer reviewer = await _unitOfWork.GetRepository<Reviewer>().GetByIdAsync(updateReviewFormDto.ReviewerId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Reviewer not found!");
        Conclude conclude = await _unitOfWork.GetRepository<Conclude>().GetByIdAsync(updateReviewFormDto.ConcludeId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Conclude not found!");
        _mapper.Map(updateReviewFormDto, reviewForm);
        reviewForm.UpdatedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteReviewFormAsync(int id)
    {
        Review_Form reviewForm = await _unitOfWork.GetRepository<Review_Form>().GetByIdAsync(id) ?? 
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Form not found!");
        reviewForm.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Form>().UpdateAsync(reviewForm);
        await _unitOfWork.SaveChangesAsync();
    }
    
}