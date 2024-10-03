using AutoMapper;
using Microsoft.AspNetCore.Http;
using Seminar.APPLICATION.Dtos.ProceedingsDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class ProceedingService : IProceedingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProceedingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProceedingVM> GetProceedingByIdAsync(int id)
    {
        Proceeding proceeding = await _unitOfWork.GetRepository<Proceeding>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Proceeding not found!");
        return _mapper.Map<ProceedingVM>(proceeding);
    }

    public async Task CreateProceedingAsync(CUProceedingDto proceedingDto)
    {
        Proceeding proceeding = _mapper.Map<Proceeding>(proceedingDto);
        await _unitOfWork.GetRepository<Proceeding>().InsertAsync(proceeding);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateProceedingAsync(int id, CUProceedingDto proceedingDto)
    {
        Proceeding proceeding = await _unitOfWork.GetRepository<Proceeding>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Proceeding not found!");
        _mapper.Map(proceedingDto, proceeding);
        proceeding.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Proceeding>().UpdateAsync(proceeding);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProceedingAsync(int id)
    {
        Proceeding proceeding = await _unitOfWork.GetRepository<Proceeding>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Proceeding not found!");
        proceeding.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Proceeding>().UpdateAsync(proceeding);
        await _unitOfWork.SaveChangesAsync();
    }

}