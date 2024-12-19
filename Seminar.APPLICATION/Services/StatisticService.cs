using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.StatisticDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class StatisticService : IStatisticService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    public StatisticService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
    public async Task<StatisticsVM> GetStatisticsByOrganizer(StatisticsFilterDto filterDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefault(o => o.AccountId == userId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Ban tổ chức không tồn tại. Vui lòng cung cấp ban tổ chức hợp lệ.");

        StatisticsVM statisticsVM = new StatisticsVM();

        // Query để đếm tổng số (không áp dụng filter)
        IQueryable<Competition> totalQuery = _unitOfWork.GetRepository<Competition>()
            .Entities
            .Where(c => !c.DeletedAt.HasValue && c.OrganizerId == organizer.Id);

        // Query để lọc danh sách (có áp dụng filter)
        IQueryable<Competition> filteredQuery = _unitOfWork.GetRepository<Competition>()
            .Entities
            .Where(c => !c.DeletedAt.HasValue && c.OrganizerId == organizer.Id);

        if (filterDto.CompetitionId.HasValue)
            filteredQuery = filteredQuery.Where(c => c.Id == filterDto.CompetitionId);

        if (filterDto.Year.HasValue && filterDto.Year.Value > 0)
        {
            totalQuery = totalQuery.Where(c => c.DateStart != null && c.DateStart.Value.Year == filterDto.Year.Value);
        }

        // Lọc theo Discipline
        if (filterDto.DisciplineId.HasValue && filterDto.DisciplineId > 0)
        {
            filteredQuery = filteredQuery.Where(c => c.ResearchTopics
                .Any(rt => rt.DisciplineId == filterDto.DisciplineId.Value));
        }

        List<Competition> competitions = await filteredQuery.ToListAsync();

        //Competition Statistics
        statisticsVM.CompetitionStatistics = new CompetitionStatistics
        {
            TotalCompetition = await totalQuery.CountAsync(),
            Competition = _mapper.Map<List<CompetitionVM>>(competitions),
            UpcomingCompetition = await totalQuery.CountAsync(c => c.DateStart > DateTime.Now && c.DeletedAt == null),
            OngoingCompetition = await totalQuery.CountAsync(c => c.DateStart <= DateTime.Now && c.DateEnd >= DateTime.Now && c.DeletedAt == null),
            FinishedCompetition = await totalQuery.CountAsync(c => c.DateEnd < DateTime.Now && c.DeletedAt == null),
        };

        //Registration Form Statistics
        statisticsVM.RegistrationFormStatistics = new RegistrationFormStatistics
        {
            TotalRegistrationForm = await filteredQuery.SelectMany(c => c.RegistrationForms).CountAsync(rf => rf.DeletedAt == null),
            ApprovedRegistrationForm = await filteredQuery.SelectMany(c => c.RegistrationForms).CountAsync(rf => rf.IsAccepted == (int)RegistrationFormEnum.Approved && rf.DeletedAt == null),
            PendingRegistrationForm = await filteredQuery.SelectMany(c => c.RegistrationForms).CountAsync(rf => rf.IsAccepted == (int)RegistrationFormEnum.Pending && rf.DeletedAt == null),
            RejectedRegistrationForm = await filteredQuery.SelectMany(c => c.RegistrationForms).CountAsync(rf => rf.IsAccepted == (int)RegistrationFormEnum.Rejected && rf.DeletedAt == null),
            SuccessfulRegistrationRate = await filteredQuery
                .SelectMany(c => c.RegistrationForms)
                .CountAsync(rf => rf.IsAccepted == (int)RegistrationFormEnum.Approved && rf.DeletedAt == null) /
                (double)await filteredQuery.SelectMany(c => c.RegistrationForms).CountAsync(rf => rf.DeletedAt == null) * 100
        };

        //Author Statistics
        statisticsVM.AuthorStatistics = new AuthorStatistics
        {
            TotalAuthor = await filteredQuery.SelectMany(c => c.ResearchTopics)
                                            .SelectMany(rt => rt.Author_ResearchTopics)
                                            .Where(art => art.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR && art.DeletedAt == null)
                                            .Select(art => art.AuthorId).Distinct().CountAsync(),
            TotalCoAuthor = await filteredQuery.SelectMany(c => c.ResearchTopics)
                                            .SelectMany(rt => rt.Author_ResearchTopics)
                                            .Where(art => art.RoleName == CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR && art.DeletedAt == null)
                                            .Select(art => art.AuthorId).Distinct().CountAsync(),
        };

        //Review Committee Statistics
        statisticsVM.ReviewCommitteeStatistics = new ReviewCommitteeStatistics
        {
            TotalReviewCommittee = await filteredQuery.SelectMany(c => c.Review_Committees).CountAsync(rc => rc.DeletedAt == null),
            TotalReviewer = await filteredQuery.SelectMany(c => c.Review_Committees).CountAsync(rc => rc.DeletedAt == null),
            ReviewerParticipationRate = await CalculateReviewerParticipationRate(filteredQuery)
        };

        //Article Statistics
        statisticsVM.ArticleStatistics = new ArticleStatistics
        {
            TotalArticle = await filteredQuery.SelectMany(c => c.ResearchTopics).CountAsync(rt => rt.ArticleId.HasValue && rt.DeletedAt == null),
            Article = await filteredQuery
            .SelectMany(c => c.ResearchTopics)
            .Where(rt => rt.DeletedAt == null && rt.ArticleId.HasValue)
            .Select(rt => _mapper.Map<ArticleVM>(rt.Articles))
            .ToListAsync()
        };

        //Discipline Statistics
        var totalResearchTopics = await filteredQuery
            .SelectMany(c => c.ResearchTopics)
            .Where(rt => rt.DeletedAt == null)
            .CountAsync();

        // Lấy thống kê theo từng discipline
        var disciplineDetails = await filteredQuery
            .SelectMany(c => c.ResearchTopics).Where(rt => rt.DeletedAt == null)
            .GroupBy(rt => rt.Disciplines)
            .Select(g => new DisciplineDetailStatistics
            {
                Discipline = _mapper.Map<DisciplineVM>(g.Key),
                Count = g.Count(),
                Percent = totalResearchTopics > 0
                    ? Math.Round((g.Count() / (double)totalResearchTopics) * 100, 2)
                    : 0
            }).ToListAsync();

        statisticsVM.DisciplineStatistics = new DisciplineStatistics
        {
            TotalDiscipline = disciplineDetails.Count,
            DisciplineDetailStatistics = disciplineDetails
        };

        //Research Field Statistics

        statisticsVM.ResearchFieldStatistics = new ResearchFieldStatistics
        {
            // 1. Tổng số chủ đề nghiên cứu
            TotalResearchTopic = totalResearchTopics,

            // 2. Tổng số đề tài được phản biện thành công
            TotalSuccessfulReviewedTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Approved),

            // 3. Tổng số đề tài chưa được phản biện
            TotalPendingReviewTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Pending),

            // 4. Tỉ lệ đề tài phản biện thành công
            SuccessfulReviewRate = totalResearchTopics > 0
                ? Math.Round((double)filteredQuery
                    .SelectMany(c => c.ResearchTopics)
                    .Where(rt => rt.DeletedAt == null)
                    .Count(rt => rt.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Approved)
                    / totalResearchTopics * 100, 2) : 0,

            // 5. Tổng số đề tài bị phản biện từ chối
            TotalRejectedReviewTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Rejected),

            // 6. Tổng số đề tài được khoa phê duyệt
            TotalFacultyApprovedTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.Acceptance.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Approved),

            // 7. Tổng số đề tài được khoa từ chối
            TotalFacultyRejectedTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.Acceptance.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Rejected),

            // 8. Tổng số đề tài được public
            TotalPublishedTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.Acceptance.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved),

            // 9. Tỷ lệ đề tài được public
            PublishedTopicsRate = totalResearchTopics > 0
                ? Math.Round((double)filteredQuery
                    .SelectMany(c => c.ResearchTopics)
                    .Where(rt => rt.DeletedAt == null)
                    .Count(rt => rt.Acceptance.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved)
                    / totalResearchTopics * 100, 2)
                : 0,

            // 10. Tổng số đề tài chưa được public
            TotalPendingPublishedTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.Acceptance.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Pending),

            // 11. Tổng số đề tài không được public
            TotalRejectedPublishedTopics = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Count(rt => rt.Acceptance.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Rejected),

            // 12. Tổng số đề tài chưa được khoa phê duyệt
            TotalFacultyPendingReviewTopics = await filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null)
                .Where(rt => rt.Acceptance.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Pending)
                .CountAsync(),

            // 13. Tỷ lệ đề t��i được khoa phê duyệt
            FacultyApprovedTopicsRate = totalResearchTopics > 0
                ? Math.Round((double)filteredQuery
                    .SelectMany(c => c.ResearchTopics)
                    .Where(rt => rt.DeletedAt == null)
                    .Count(rt => rt.Acceptance.FacultyAcceptedStatus == (int)FacultyAcceptedStatusEnum.Approved)
                    / totalResearchTopics * 100, 2)
                : 0,
             // 14.Tổng số tiền của những đề tài được nghiệm thu
            TotalBudgets = filteredQuery
                .SelectMany(c => c.ResearchTopics)
                .Where(rt => rt.DeletedAt == null && rt.Acceptance.AcceptedForPublicationStatus == (int)AcceptedForPublicationStatusEnum.Approved)
                .Sum(rt => rt.Budget),

        };
        return statisticsVM;
    }

    private async Task<double> CalculateReviewerParticipationRate(IQueryable<Competition> query)
    {
        double totalReviewers = await query
            .SelectMany(c => c.Review_Committees)
            .CountAsync(rc => rc.DeletedAt == null);

        if (totalReviewers == 0) return 0;

        double activeReviewers = await query
            .SelectMany(c => c.Review_Committees)
            .CountAsync(rc => rc.DeletedAt == null && rc.ResearchTopics.Any());

        return (activeReviewers / totalReviewers) * 100;
    }
}
