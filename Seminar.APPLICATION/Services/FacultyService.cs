using AutoMapper;
using Microsoft.AspNetCore.Http;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class FacultyService : IFacultyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FacultyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IList<FacultyVM>> GetAllFacultyAsync()
    {
        IList<Faculty> faculties = await _unitOfWork.GetRepository<Faculty>().GetAllAsync();
        return _mapper.Map<IList<FacultyVM>>(faculties);
    }

    public async Task<FacultyVM> GetFacultyByIdAsync(int id)
    {
        Faculty faculty = await _unitOfWork.GetRepository<Faculty>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Faculty not found!");
        return _mapper.Map<FacultyVM>(faculty);
    }
    
}