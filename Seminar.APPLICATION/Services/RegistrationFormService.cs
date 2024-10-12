using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class RegistrationFormService : IRegistrationFormService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegistrationFormService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RegistrationFormVM> GetRegistrationFormByIdAsync(int id)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().Entities.Include(x => x.Author).Include(x => x.Competition).FirstOrDefaultAsync(x => x.Id == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        RegistrationFormVM registrationFormVM = _mapper.Map<RegistrationFormVM>(registrationForm);
        registrationFormVM.AuthorName = registrationForm.Author.Name;
        registrationFormVM.CompetitionName = registrationForm.Competition.CompetitionName;
        return registrationFormVM;
    }

    public async Task CreateRegistrationFormAsync(CURegistrationFormDto dto)
    {
        RegistrationForm registrationForm = _mapper.Map<RegistrationForm>(dto);
        Author author = await _unitOfWork.GetRepository<Author>().GetByIdAsync(dto.AuthorId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(dto.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
        await _unitOfWork.GetRepository<RegistrationForm>().InsertAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateRegistrationFormAsync(int id, CURegistrationFormDto dto)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        Author author = await _unitOfWork.GetRepository<Author>().GetByIdAsync(dto.AuthorId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(dto.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
        registrationForm.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<RegistrationForm>().UpdateAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteRegistrationFormAsync(int id)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        await _unitOfWork.GetRepository<RegistrationForm>().DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    } 
}