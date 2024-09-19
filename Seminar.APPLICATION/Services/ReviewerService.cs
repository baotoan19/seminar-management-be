using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.APPLICATION.Interfaces;
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

    public ReviewerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewerService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Reviewer> CreateReviewerAsync(CreateReviewerDto createReviewerDto)
    {
        Reviewer? existsReviewer = await _unitOfWork.GetRepository<Reviewer>().Entities.FirstOrDefaultAsync(r => r.AccountId == createReviewerDto.AccountId);
        if (existsReviewer != null)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Reviewer is existed!");
        }
        Reviewer reviewer = _mapper.Map<Reviewer>(createReviewerDto);
        await _unitOfWork.GetRepository<Reviewer>().InsertAsync(reviewer);
        await _unitOfWork.SaveChangesAsync();
        return reviewer;
    }
}
