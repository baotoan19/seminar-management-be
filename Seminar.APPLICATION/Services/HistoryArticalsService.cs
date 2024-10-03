using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Dtos.HistoryArticasDtos;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Interfaces;

public class HistoryArticalsService : IHistoryArticalsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HistoryArticalsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<HistoryArticalsVM> GetHistoryUpdateArticalsByIdAsync(int id)
    {
        History_Update_Artical historyArticals = await _unitOfWork.GetRepository<History_Update_Artical>().Entities.Include(a => a.Artical).FirstOrDefaultAsync(a => a.Id == id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "History Artical not found!");
        HistoryArticalsVM historyArticalsVM = _mapper.Map<HistoryArticalsVM>(historyArticals);
        historyArticalsVM.Title = historyArticals.Artical.Title;
        return historyArticalsVM;
    }

    public async Task CreateHistoryUpdateArticalsAsync(CreateHistoryArticalsDto createHistoryArticalsDto)
    {
        Artical artical = await _unitOfWork.GetRepository<Artical>().GetByIdAsync(createHistoryArticalsDto.ArticalId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        History_Update_Artical historyArticals = _mapper.Map<History_Update_Artical>(createHistoryArticalsDto);
        await _unitOfWork.GetRepository<History_Update_Artical>().InsertAsync(historyArticals);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteHistoryUpdateArticalsAsync(int id)
    {
        History_Update_Artical historyArtical = await _unitOfWork.GetRepository<History_Update_Artical>().GetByIdAsync(id)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "History Artical not found!");
        bool isLinkedToReviewForm = await _unitOfWork.GetRepository<Review_Form>().Entities
            .AnyAsync(rf => rf.HistoryId == id);
        if (isLinkedToReviewForm)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Cannot delete History Artical because it is linked to a Review Form.");
        }
        await _unitOfWork.GetRepository<History_Update_Artical>().DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}

