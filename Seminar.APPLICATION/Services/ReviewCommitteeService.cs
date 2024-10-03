using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class ReviewCommitteeService : IReviewCommitteeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewCommitteeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    
    public async Task<ReviewCommitteeVM> GetReviewCommitteeByIdAsync(int id)
    {
        Review_Committee reviewCommittee = await _unitOfWork.GetRepository<Review_Committee>().Entities.Include(x => x.Conference).FirstOrDefaultAsync(x => x.Id == id) ??throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Committee not found!");
        ReviewCommitteeVM reviewCommitteeVM = _mapper.Map<ReviewCommitteeVM>(reviewCommittee);
        reviewCommitteeVM.ConferenceName = reviewCommittee.Conference.ConferenceName;
        return reviewCommitteeVM;
    }

    public async Task CreateReviewCommitteeAsync(CUReviewCommitteeDto dto)
    {
        Review_Committee reviewCommittee = _mapper.Map<Review_Committee>(dto);
        Conference conference = await _unitOfWork.GetRepository<Conference>().GetByIdAsync(dto.ConferenceId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Conference not found!");
        await _unitOfWork.GetRepository<Review_Committee>().InsertAsync(reviewCommittee);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateReviewCommitteeAsync(int id, CUReviewCommitteeDto dto)
    {
        Review_Committee reviewCommittee = await _unitOfWork.GetRepository<Review_Committee>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Committee not found!");
        Conference conference = await _unitOfWork.GetRepository<Conference>().GetByIdAsync(dto.ConferenceId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Conference not found!");
        _mapper.Map(dto, reviewCommittee);
        reviewCommittee.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Committee>().UpdateAsync(reviewCommittee);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteReviewCommitteeAsync(int id)
    {
        Review_Committee reviewCommittee = await _unitOfWork.GetRepository<Review_Committee>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Committee not found!");
        reviewCommittee.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Committee>().UpdateAsync(reviewCommittee);
        await _unitOfWork.SaveChangesAsync();
    }
}