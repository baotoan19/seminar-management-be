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
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Services;

public class RegistrationFormService : IRegistrationFormService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFirebaseService _firebaseService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegistrationFormService(IUnitOfWork unitOfWork, IMapper mapper, IFirebaseService firebaseService, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _firebaseService = firebaseService;
    }
    
    public async Task<PaginatedList<RegistrationFormVM>> GetAllByCompetitionIdAsync(int competitionId, int index, int pageSize, string idSearch,string internalCodeSearch, int isAccepted)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(competitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
        if (competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not allowed to access this competition!");
        }
        if  (competitionId <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid competition id");
        }
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }
        IQueryable<RegistrationForm> query = _unitOfWork.GetRepository<RegistrationForm>().Entities
            .Where(x => x.DeletedAt == null && x.CompetitionId == competitionId)
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

        if (!string.IsNullOrEmpty(internalCodeSearch))
        {
            query = query.Where(x => x.Author != null && x.Author.InternalCode != null && x.Author.InternalCode.Contains(internalCodeSearch));
        }
        
        if (isAccepted == (int)RegistrationFormEnum.Pending || isAccepted == (int)RegistrationFormEnum.Approved || isAccepted == (int)RegistrationFormEnum.Rejected || isAccepted == (int)RegistrationFormEnum.All)
        {
            if (isAccepted != (int)RegistrationFormEnum.All)
            {
                query = query.Where(x => x.IsAccepted == isAccepted);
            }
        }
        else
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid isAccepted value");
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
        List<RegistrationForm> registrationForms = await _unitOfWork.GetRepository<RegistrationForm>().Entities.Where(x => x.AuthorId == author.Id && x.DeletedAt == null).ToListAsync();
        List<RegistrationFormVM> responeItems = _mapper.Map<List<RegistrationFormVM>>(registrationForms);
        return responeItems;
    }

    public async Task<RegistrationFormVM> GetRegistrationFormByIdAsync(int id)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        RegistrationFormVM registrationFormVM = _mapper.Map<RegistrationFormVM>(registrationForm);
        return registrationFormVM;
    }

    public async Task CreateRegistrationFormAsync(CreateRegistrationFormDto dto)
    {
        int accountId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == accountId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(dto.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
        DateTime now = DateTime.Now;
        if (now < competition.DateStart || now > competition.DateEnd)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Registration period for this competition is not active!");
        }
        RegistrationForm registrationForm = _mapper.Map<RegistrationForm>(dto);
        registrationForm.AuthorId = author.Id;
        string filePath = await _firebaseService.UploadFileAsync(dto.FilePath, FirebaseConstants.RegistrationFormsFolder);
        registrationForm.FilePath = filePath;
        registrationForm.IsAccepted = (int)RegistrationFormEnum.Pending;
        await _unitOfWork.GetRepository<RegistrationForm>().InsertAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateRegistrationFormAsync(int id, UpdateRegistrationFormDto dto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        if (registrationForm.Competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not allowed to update this registration form!");
        }
        if (dto.IsAccepted != (int)RegistrationFormEnum.Approved && dto.IsAccepted != (int)RegistrationFormEnum.Rejected)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid is accepted value!");
        }
        _mapper.Map(dto, registrationForm);
        registrationForm.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<RegistrationForm>().UpdateAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteRegistrationFormAsync(int id)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Registration Form not found!");
        if (registrationForm.Competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not allowed to delete this registration form!");
        }
        registrationForm.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<RegistrationForm>().UpdateAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }
}