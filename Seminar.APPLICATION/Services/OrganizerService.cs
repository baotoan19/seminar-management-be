using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class OrganizerService : IOrganizerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrganizerService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrganizerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrganizerService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Organizer> CreateOrganizerAsync(CreateOrganizerDto createOrganizerDto)
    {
        Organizer? existsOrganizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == createOrganizerDto.AccountId);
        if (existsOrganizer != null)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Organizer is existed!");
        }

        Organizer organizer = _mapper.Map<Organizer>(createOrganizerDto);
        await _unitOfWork.GetRepository<Organizer>().InsertAsync(organizer);
        await _unitOfWork.SaveChangesAsync();
        return organizer;
    }

    public async Task<OrganizerVM> GetOrganizerInforAsync(int id)
    {
        Organizer? organizer = await _unitOfWork.GetRepository<Organizer>().Entities.Include(o => o.Account).Include(o => o.Faculty).FirstOrDefaultAsync(o => o.AccountId == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        OrganizerVM organizerVM = _mapper.Map<OrganizerVM>(organizer);
        organizerVM.FacultyName = organizer.Faculty?.FacultyName ?? null;
        organizerVM.Email = organizer.Account.Email;
        return organizerVM;
    }

    public async Task<Organizer> UpdateOrganizerAsync(int id, UpdateOrganizerDto updateOrganizerDto)
    {
        Organizer? organizer = await _unitOfWork.GetRepository<Organizer>().Entities.Include(o => o.Account).Include(o => o.Faculty).FirstOrDefaultAsync(o => o.AccountId == id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        _mapper.Map(updateOrganizerDto, organizer);
        organizer.Account.Email = updateOrganizerDto.Email;
        organizer.Account.UpdatedAt = DateTime.Now;
        organizer.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Organizer>().UpdateAsync(organizer);
        await _unitOfWork.SaveChangesAsync();
        return organizer;
    }
}
