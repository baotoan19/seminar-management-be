using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AcceptanceDtos;
using Seminar.APPLICATION.Dtos.ReviewAcceptanceDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Services;

public class AcceptanceService : IAcceptanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    public AcceptanceService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
    public async Task<PaginatedList<AcceptanceVM>> GetAllAcceptances(int index, int pageSize, int idSearch, string nameSearch, int facultyAcceptedStatus, int acceptedForPublicationStatus, int competitionId, int facultyId)
    {
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }

        IQueryable<Acceptance> query = _unitOfWork.GetRepository<Acceptance>().Entities
            .Where(a => a.DeletedAt == null)
            .OrderByDescending(a => a.CreatedAt);

        if (idSearch != 0)
        {
            query = query.Where(a => a.Id == idSearch);
        }

        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(a => EF.Functions.Like(a.Name, $"%{nameSearch}%"));
        }

        switch (facultyAcceptedStatus)
        {
            case 0:
                query = query.Where(a => a.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Pending);
                break;
            case 1:
                query = query.Where(a => a.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Approved);
                break;
            case 2:
                query = query.Where(a => a.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Rejected);
                break;
            case 3:
                break;
        }

        switch (acceptedForPublicationStatus)
        {
            case 0:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Pending);
                break;
            case 1:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved);
                break;
            case 2:
                query = query.Where(a => a.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Rejected);
                break;
            case 3:
                break;
        }

        if (competitionId != 0)
        {
            query = query.Where(a => a.ResearchTopic.Competitions.Id == competitionId);
        }

        if (facultyId != 0)
        {
            query = query.Where(a => a.ResearchTopic.Competitions.Organizer.Faculty.Id == facultyId);
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<AcceptanceVM>(new List<AcceptanceVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        foreach (var acceptance in resultQuery)
        {
            acceptance.Review_Acceptances = acceptance.Review_Acceptances
                .Where(ra => ra.DeletedAt == null)
                .ToList();
        }
        List<AcceptanceVM> responeItems = _mapper.Map<List<AcceptanceVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<AcceptanceVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<AcceptanceVM> GetAcceptanceById(int id)
    {
        Acceptance acceptance = await _unitOfWork.GetRepository<Acceptance>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Acceptance not found!");
        acceptance.Review_Acceptances = acceptance.Review_Acceptances
            .Where(ra => ra.DeletedAt == null)
            .ToList();
        return _mapper.Map<AcceptanceVM>(acceptance);
    }
    private async Task<int> GetAuthorId()
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(x => x.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        return author.Id;
    }
    public async Task CreateAcceptance(CreateAcceptanceDto dto)
    {
        int authorId = await GetAuthorId();
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(dto.ResearchTopicId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        if (researchTopic.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Pending || researchTopic.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Rejected)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Research topic is not reviewed!");
        }
        if (researchTopic.ProductFilePath == null)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Research topic is not have product file!");
        }
        if (researchTopic.ReportFilePath == null)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Research topic is not have report file!");
        }
        if (researchTopic.BudgetFilePath == null)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Research topic is not have budget file!");
        }
        DateTime now = DateTime.Now;
        if (now > researchTopic.DateEnd)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Acceptance time of the research topic has ended!");
        }

        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(x => x.ResearchTopicId == dto.ResearchTopicId && x.AuthorId == authorId && x.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR && x.DeletedAt == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author is not the creator of the research topic!");
        Acceptance acceptance = _mapper.Map<Acceptance>(dto);
        acceptance.FacultyAcceptedStatus = (int)FacultyAcceptedStatusEnum.Pending;
        acceptance.AcceptedForPublicationStatus = (int)AcceptedForPublicationStatusEnum.Pending;
        await _unitOfWork.GetRepository<Acceptance>().InsertAsync(acceptance);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAcceptance(int id)
    {
        int authorId = await GetAuthorId();
        Acceptance acceptance = await _unitOfWork.GetRepository<Acceptance>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Acceptance not found!");
        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(x => x.ResearchTopicId == acceptance.ResearchTopicId && x.AuthorId == authorId && x.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR && x.DeletedAt == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author is not the creator of the research topic!");
        if (acceptance.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Acceptance is already accepted by faculty!");
        }
        if (acceptance.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Acceptance is already accepted for publication!");
        }
        acceptance.DeletedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task CreateReviewAcceptance(CreateReviewAcceptanceDto dto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities
            .FirstOrDefaultAsync(x => x.AccountId == userId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound,
                ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        Acceptance acceptance = await _unitOfWork.GetRepository<Acceptance>().GetByIdAsync(dto.AcceptanceId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound,
                ResponseCodeConstants.NOT_FOUND, "Acceptance not found!");
        if (acceptance.ResearchTopic.Competitions.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden,
                ResponseCodeConstants.FORBIDDEN,
                "You are not the organizer of this competition and you are not allowed to perform this action!");
        }
        Review_Acceptance review_Acceptance = _mapper.Map<Review_Acceptance>(dto);
        review_Acceptance.OrganizerId = organizer.Id;
        await _unitOfWork.GetRepository<Review_Acceptance>().InsertAsync(review_Acceptance);
        if (dto.IsAccepted)
        {
            acceptance.FacultyAcceptedStatus = (int)FacultyAcceptedStatusEnum.Approved;
            await _unitOfWork.GetRepository<Acceptance>().UpdateAsync(acceptance);
        }
        else
        {
            acceptance.FacultyAcceptedStatus = (int)FacultyAcceptedStatusEnum.Rejected;
            await _unitOfWork.GetRepository<Acceptance>().UpdateAsync(acceptance);
        }
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAcceptanceForPublication(int acceptanceId, UpdateAcceptanceForPublicationDto dto)
    {
        Acceptance acceptance = await _unitOfWork.GetRepository<Acceptance>().GetByIdAsync(acceptanceId)
        ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Acceptance not found!");
        if (acceptance.FacultyAcceptedStatus != (int)FacultyAcceptedStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Acceptance is not accepted by faculty!");
        }
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(acceptance.ResearchTopicId)
        ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        if (acceptance.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Acceptance is already accepted for publication!");
        }
        if (dto.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved)
        {
            researchTopic.AcceptanceApprovedStatus = (int)AcceptanceApprovedStatusEnum.Approved;
            await _unitOfWork.GetRepository<ResearchTopic>().UpdateAsync(researchTopic);
        }
        acceptance.AcceptedForPublicationStatus = dto.AcceptedForPublicationStatus;
        acceptance.DateAcceptance = DateTime.Now;
        acceptance.UpdatedAt = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
    }

}
