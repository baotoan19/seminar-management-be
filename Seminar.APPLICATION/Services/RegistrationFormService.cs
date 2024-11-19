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
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Nhóm người dùng không tồn tại. Vui lòng cung cấp nhóm người dùng hợp lệ.");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(competitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        if (competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không được phép truy cập cuộc thi này.");
        }
        if  (competitionId <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "ID cuộc thi không hợp lệ. Vui lòng cung cấp ID cuộc thi hợp lệ.");
        }
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ. Vui lòng cung cấp chỉ số và kích thước trang hợp lệ.");
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
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đăng ký không tồn tại. Vui lòng cung cấp đăng ký hợp lệ.");
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
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Giá trị isAccepted không hợp lệ. Vui lòng cung cấp giá trị hợp lệ.");
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
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại. Vui lòng cung cấp tác giả hợp lệ.");
        List<RegistrationForm> registrationForms = await _unitOfWork.GetRepository<RegistrationForm>().Entities.Where(x => x.AuthorId == author.Id && x.DeletedAt == null).ToListAsync();
        List<RegistrationFormVM> responeItems = _mapper.Map<List<RegistrationFormVM>>(registrationForms);
        return responeItems;
    }

    public async Task<RegistrationFormVM> GetRegistrationFormByIdAsync(int id)
    {
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Phiếu đăng ký không tồn tại. Vui lòng cung cấp phiếu đăng ký hợp lệ.");
        RegistrationFormVM registrationFormVM = _mapper.Map<RegistrationFormVM>(registrationForm);
        return registrationFormVM;
    }

    public async Task CreateRegistrationFormAsync(CreateRegistrationFormDto dto)
    {
        int accountId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == accountId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại. Vui lòng cung cấp tác giả hợp lệ.");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(dto.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        DateTime now = DateTime.Now;
        if (now > competition.DateEndSubmit || now < competition.DateStart)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Thời gian đăng ký cho cuộc thi này không hoạt động.");
        }
        RegistrationForm registrationForm = _mapper.Map<RegistrationForm>(dto);
        registrationForm.AuthorId = author.Id;
        registrationForm.IsAccepted = (int)RegistrationFormEnum.Pending;
        await _unitOfWork.GetRepository<RegistrationForm>().InsertAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateRegistrationFormAsync(int id, UpdateRegistrationFormDto dto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Nhóm người dùng không tồn tại. Vui lòng cung cấp nhóm người dùng hợp lệ.");
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Phiếu đăng ký không tồn tại. Vui lòng cung cấp phiếu đăng ký hợp lệ.");
        if (registrationForm.Competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không được phép cập nhật phiếu đăng ký này.");
        }
        if (dto.IsAccepted != (int)RegistrationFormEnum.Approved && dto.IsAccepted != (int)RegistrationFormEnum.Rejected)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Giá trị isAccepted không hợp lệ. Vui lòng cung cấp giá trị hợp lệ.");
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
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Nhóm người dùng không tồn tại. Vui lòng cung cấp nhóm người dùng hợp lệ.");
        RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Phiếu đăng ký không tồn tại. Vui lòng cung cấp phiếu đăng ký hợp lệ.");
        if (registrationForm.Competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không được phép xóa phiếu đăng ký này.");
        }
        registrationForm.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<RegistrationForm>().UpdateAsync(registrationForm);
        await _unitOfWork.SaveChangesAsync();
    }
}