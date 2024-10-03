using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class ConcludeService : IConcludeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ConcludeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IList<ConcludeVM>> GetAllAsync()
    {
        IList<Conclude> concludes = await _unitOfWork.GetRepository<Conclude>().GetAllAsync();
        IList<ConcludeVM> concludeVMs = _mapper.Map<IList<ConcludeVM>>(concludes);
        return concludeVMs.ToList();
    }

    public async Task<ConcludeVM> GetByIdAsync(int id)
    {
        Conclude conclude = await _unitOfWork.GetRepository<Conclude>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Conclude not found!");
        return _mapper.Map<ConcludeVM>(conclude);
    }
}