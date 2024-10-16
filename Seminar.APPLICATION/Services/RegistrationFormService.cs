using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.RegistrationFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Services;

public class RegistrationFormService : IRegistrationFormService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegistrationFormService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PaginatedList<RegistrationFormVM>> GetPagedAsync(int index, int pageSize, string idSearch, string nameSearch)
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }
        IQueryable<RegistrationForm> query = _unitOfWork.GetRepository<RegistrationForm>().Entities
            .Include(x => x.Author)
            .Include(x => x.Competition)
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt);

        if (!string.IsNullOrEmpty(idSearch))
        {
            query = query.Where(x => x.Id.ToString().Contains(idSearch));
            bool isInt = int.TryParse(idSearch, out int idInt);
            if (!isInt)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
            }
        }

        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(x => x.Competition.CompetitionName.Contains(nameSearch));
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<RegistrationFormVM>(new List<RegistrationFormVM>(), 0, index, pageSize);
        }

        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        List<RegistrationFormVM> responeItems = _mapper.Map<List<RegistrationFormVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<RegistrationFormVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }

    public async Task<List<RegistrationFormVM>> GetAllByAuthorIdAsync()
    {
        int accountId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == accountId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        List<RegistrationForm> registrationForms = await _unitOfWork.GetRepository<RegistrationForm>().Entities.Include(x => x.Competition).Where(x => x.AuthorId == author.Id && x.DeletedAt == null).ToListAsync();
        List<RegistrationFormVM> responeItems = _mapper.Map<List<RegistrationFormVM>>(registrationForms);
        return responeItems;
    }

    public async Task<RegistrationFormVM> GetRegistrationFormByIdAsync(int id)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().Entities.Include(x => x.Author).Include(x => x.Competition).FirstOrDefaultAsync(x => x.Id == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        RegistrationFormVM registrationFormVM = _mapper.Map<RegistrationFormVM>(registrationForm);
        return registrationFormVM;
    }

    public async Task CreateRegistrationFormAsync(CreateRegistrationFormDto dto)
    {
        int accountId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == accountId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        RegistrationForm registrationForm = _mapper.Map<RegistrationForm>(dto);
        registrationForm.AuthorId = author.Id;
        registrationForm.IsAccepted = 1; // 1: Pending, 2: Accepted, 3: Rejected
        await _unitOfWork.GetRepository<RegistrationForm>().InsertAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateRegistrationFormAsync(int id, UpdateRegistrationFormDto dto)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        _mapper.Map(dto, registrationForm);
        registrationForm.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<RegistrationForm>().UpdateAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteRegistrationFormAsync(int id)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        registrationForm.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<RegistrationForm>().UpdateAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }
}