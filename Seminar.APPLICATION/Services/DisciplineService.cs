using AutoMapper;
using Microsoft.AspNetCore.Http;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Interfaces;

public class DisciplineService : IDisciplineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DisciplineService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IList<DisciplineVM>> GetAllDisciplineAsync()
    {
        IList<Discipline> disciplines = await _unitOfWork.GetRepository<Discipline>().GetAllAsync();
        return _mapper.Map<IList<DisciplineVM>>(disciplines);
    }

    public async Task<DisciplineVM> GetDisciplineByIdAsync(int id)
    {
        Discipline discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        return _mapper.Map<DisciplineVM>(discipline);
    }
}