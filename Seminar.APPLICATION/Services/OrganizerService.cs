using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
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

    public OrganizerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrganizerService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
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
}