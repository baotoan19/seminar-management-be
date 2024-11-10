using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.CORE.Utils;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;
namespace Seminar.APPLICATION.Services;

public class OrganizerService : IOrganizerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrganizerService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    public OrganizerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrganizerService> logger, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
    }
    //Organizer
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
        // OrganizerVM organizerVM = _mapper.Map<OrganizerVM>(organizer);
        // organizerVM.FacultyName = organizer.Faculty?.FacultyName ?? null;
        // organizerVM.Email = organizer.Account.Email;
        OrganizerVM organizerVM = new OrganizerVM
        {
            Id = organizer.Id,
            Name = organizer.Name ?? "Unknown",
            Email = organizer.Account.Email ?? "Unknown",
            NumberPhone = organizer.NumberPhone ?? "Unknown",
            Description = organizer.Description ?? "Unknown",
            FacultyId = organizer.FacultyId ?? 0,
            FacultyName = organizer.Faculty?.FacultyName ?? "Unknown",
            AccountId = organizer.AccountId
        };
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

    //Review Committee
    public async Task<PaginatedList<ReviewCommitteeVM>> GetReviewCommitteeByCompetitionIdAsync(int competitionId, int page, int pageSize, int idSearch, string nameSearch)
    {
        Competition? competition = await _unitOfWork.GetRepository<Competition>()
            .Entities.FirstOrDefaultAsync(c => c.Id == competitionId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");

        var query = _unitOfWork.GetRepository<Review_Committee>().Entities
            .Where(rc => rc.CompetitionId == competitionId && rc.DeletedAt == null)
            .OrderByDescending(rc => rc.CreatedAt)
            .Select(rc => new ReviewCommitteeVM
            {
                Id = rc.Id,
                ReviewCommitteeName = rc.ReviewCommitteeName,
                CompetitionId = rc.CompetitionId ?? 0,
                CompetitionName = rc.Competitions.CompetitionName,
                ReviewBoardMembers = rc.Review_Board_Members
                    .Where(rbm => rbm.IsStatus == true && rbm.DeletedAt == null)
                    .Select(rbm => new ReviewBoardMemberVM
                    {
                        Id = rbm.Id,
                        Description = rbm.Description ?? "Unknown",
                        IsStatus = rbm.IsStatus,
                        Name = rbm.Reviewer.Name ?? "Unknown",
                        NumberPhone = rbm.Reviewer.NumberPhone ?? "Unknown",
                        DateOfBirth = rbm.Reviewer.DateOfBirth,
                        AccountId = rbm.Reviewer.AccountId,
                        FacultyId = rbm.Reviewer.FacultyId ?? 0,
                        FacultyName = rbm.Reviewer.Faculty.FacultyName ?? "Unknown",
                        Email = rbm.Reviewer.Email ?? "Unknown"
                    }).ToList()
            });

        // Thêm điều kiện tìm kiếm
        if (idSearch != 0)
        {
            query = query.Where(rc => rc.Id == idSearch);
        }
        if (!string.IsNullOrEmpty(nameSearch))
        {
            query = query.Where(rc => rc.ReviewCommitteeName.Contains(nameSearch));
        }

        query = query.OrderByDescending(rc => rc.Id);

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ReviewCommitteeVM>(new List<ReviewCommitteeVM>(), 0, page, pageSize);
        }

        var resultQuery = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PaginatedList<ReviewCommitteeVM>(resultQuery, totalCount, page, pageSize);
    }
    public async Task CreateReviewCommitteeAsync(CreateReviewCommitteeDto createReviewCommitteeDto)
    {
        var strategy = _unitOfWork.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Thêm Review Committee
                    Competition? competition = await _unitOfWork.GetRepository<Competition>().Entities
                    .FirstOrDefaultAsync(c => c.Id == createReviewCommitteeDto.CompetitionId) ??
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");
                    Review_Committee reviewCommittee = _mapper.Map<Review_Committee>(createReviewCommitteeDto);
                    await _unitOfWork.GetRepository<Review_Committee>().InsertAsync(reviewCommittee);
                    await _unitOfWork.SaveChangesAsync();
                    //Thêm Account
                    if (createReviewCommitteeDto.ReviewBoardMembers != null)
                    {
                        // Kiểm tra email trùng lặp
                        await ValidateReviewerEmailsAsync(createReviewCommitteeDto.ReviewBoardMembers);

                        foreach (ReviewBoardMemberDto reviewBoardMember in createReviewCommitteeDto.ReviewBoardMembers)
                        {
                            int reviewerId = await CreateOrUpdateReviewerAsync(reviewBoardMember, reviewCommittee);
                            // Thêm Review_Board_Member
                            Review_Board_Member newReviewBoardMember = new Review_Board_Member
                            {
                                ReviewerId = reviewerId,
                                ReviewCommitteeId = reviewCommittee.Id,
                                Description = reviewBoardMember?.Description ?? "Unknown",
                                IsStatus = true
                            };
                            await _unitOfWork.GetRepository<Review_Board_Member>().InsertAsync(newReviewBoardMember);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollBackAsync();
                    throw;
                }
            }
        });
    }
    private async Task ValidateReviewerEmailsAsync(IEnumerable<ReviewBoardMemberDto> reviewBoardMembers)
    {
        var processedEmails = new HashSet<string>();
        foreach (ReviewBoardMemberDto reviewBoardMember in reviewBoardMembers)
        {
            // Kiểm tra email trùng lặp trong danh sách
            if (!processedEmails.Add(reviewBoardMember.Email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Review board member email is duplicated!");
            }

            // Kiểm tra email có tồn tại trong hệ thống không và vai trò của tài khoản
            Account? account = await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(a => a.Email == reviewBoardMember.Email);
            if (account != null && (account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR || account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.ORGANIZER))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Email is already associated with system!");
            }
        }
    }
    private async Task<int> CreateOrUpdateReviewerAsync(ReviewBoardMemberDto reviewBoardMember, Review_Committee reviewCommittee)
    {
        // Kiểm tra account có tồn tại
        Reviewer? existingReviewer = await _unitOfWork.GetRepository<Reviewer>().Entities
            .FirstOrDefaultAsync(r => r.Email == reviewBoardMember.Email);

        if (existingReviewer == null)
        {
            // Tạo tài khoản mới nếu không tồn tại
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            Role reviewerRole = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.REVIEWER) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found!");
            Account newAccount = new Account
            {
                Email = reviewBoardMember.Email,
                RoleId = reviewerRole.Id,
                Password = passwordHasher.HashPassword(new Account(), "Huit@1245"),
                IsSuspended = false,
            };

            await _unitOfWork.GetRepository<Account>().InsertAsync(newAccount);
            await _unitOfWork.SaveChangesAsync();

            Reviewer newReviewer = _mapper.Map<Reviewer>(reviewBoardMember);
            newReviewer.AccountId = newAccount.Id;
            await _unitOfWork.GetRepository<Reviewer>().InsertAsync(newReviewer);
            await _unitOfWork.SaveChangesAsync();
            await _emailService.SendReviewerAccountInfoEmail(reviewBoardMember, reviewCommittee.ReviewCommitteeName);
            return newReviewer.Id;
        }
        else
        {
            Review_Board_Member? existingReviewBoardMember = await _unitOfWork.GetRepository<Review_Board_Member>().Entities
                .FirstOrDefaultAsync(r => r.ReviewerId == existingReviewer.Id && r.ReviewCommitteeId == reviewCommittee.Id);

            if (existingReviewBoardMember != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Review board member is existed!");
            }

            return existingReviewer.Id;
        }
    }

}


