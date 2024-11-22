using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.CompetitionDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;
namespace Seminar.APPLICATION.Services;

public class CompetitionService : ICompetitionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CompetitionService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PaginatedList<CompetitionVM>> GetAllCompetitionByOrganizerIdAsync(int index, int pageSize, string nameSearch)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer? organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Ban tổ chức không tồn tại. Vui lòng cung cấp tổ chức hợp lệ.");

        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ");
        }

        IQueryable<Competition> query = _unitOfWork.GetRepository<Competition>().Entities
            .Include(c => c.Organizer)
            .Where(c => c.DeletedAt == null && c.OrganizerId == organizer.Id)
            .OrderByDescending(c => c.CreatedAt);

        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(c => EF.Functions.Like(c.CompetitionName, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại!");
            }
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<CompetitionVM>(new List<CompetitionVM>(), 0, index, pageSize);
        }

        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        List<CompetitionVM> responeItems = _mapper.Map<List<CompetitionVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<CompetitionVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<PaginatedList<CompetitionVM>> GetAllCompetitionAsync(int index, int pageSize, string nameSearch, string organizerName, int facultyId)
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ");
        }

        IQueryable<Competition> query = _unitOfWork.GetRepository<Competition>().Entities
            .Include(c => c.Organizer)
            .Where(c => c.DeletedAt == null)
            .OrderByDescending(c => c.CreatedAt);

        if (!string.IsNullOrEmpty(organizerName))
        {
            query = query.Where(c => EF.Functions.Like(c.Organizer.Name, $"%{organizerName}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại!");
            }
        }

        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(c => EF.Functions.Like(c.CompetitionName, $"%{nameSearch}%"));
            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại!");
            }
        }

        if (facultyId != 0)
        {
            query = query.Where(c => c.Organizer.Faculty.Id == facultyId);
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<CompetitionVM>(new List<CompetitionVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        List<CompetitionVM> responeItems = _mapper.Map<List<CompetitionVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<CompetitionVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<CompetitionVM> GetCompetitionByIdAsync(int id)
    {
        Competition competition = await _unitOfWork.GetRepository<Competition>().Entities.Include(c => c.Organizer).FirstOrDefaultAsync(c => c.Id == id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        return _mapper.Map<CompetitionVM>(competition);
    }
    public async Task CreateCompetitionAsync(CreateCompetitionDto createCompetitionDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer? organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Ban tổ chức không tồn tại. Vui lòng cung cấp ban tổ chức hợp lệ.");
        if(createCompetitionDto.DateEndSubmit > createCompetitionDto.DateEnd || createCompetitionDto.DateEndSubmit < createCompetitionDto.DateStart)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Ngày nộp bài phải nhỏ hơn ngày kết thúc và lớn hơn ngày bắt đầu!");
        }
        Competition competition = _mapper.Map<Competition>(createCompetitionDto);
        competition.OrganizerId = organizer.Id;
        await _unitOfWork.GetRepository<Competition>().InsertAsync(competition);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateCompetitionAsync(int id, UpdateCompetitionDto updateCompetitionDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer? organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Ban tổ chức không tồn tại. Vui lòng cung cấp ban tổ chức hợp lệ.");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        if (competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không được phép cập nhật cuộc thi này!");
        }
        if(updateCompetitionDto.DateEndSubmit > updateCompetitionDto.DateEnd || updateCompetitionDto.DateEndSubmit < updateCompetitionDto.DateStart)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Ngày nộp bài phải nhỏ hơn ngày kết thúc và lớn hơn ngày bắt đầu!");
        }
        _mapper.Map(updateCompetitionDto, competition);
        competition.UpdatedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteCompetitionAsync(int id)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer? organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Ban tổ chức không tồn tại. Vui lòng cung cấp ban tổ chức hợp lệ.");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        if (competition.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không được phép xóa cuộc thi này!");
        }
        competition.DeletedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateDateEndCompetitionAsync(int id, UpdateDateEndCompetitionDto updateDateEndCompetitionDto)
    {
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        competition.DateEnd = competition.DateEnd?.AddMonths(updateDateEndCompetitionDto.Month);
        competition.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Competition>().UpdateAsync(competition);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateDateSubmitCompetitionAsync(int id, UpdateDateSubmitCompetitionDto updateDateSubmitCompetitionDto)
    {
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        competition.DateEndSubmit = competition.DateEndSubmit?.AddMonths(updateDateSubmitCompetitionDto.Month);
        if(competition.DateEndSubmit > competition.DateEnd)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Ngày nộp bài phải nhỏ hơn ngày kết thúc!");
        }
        competition.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Competition>().UpdateAsync(competition);
        await _unitOfWork.SaveChangesAsync();
    }
}
