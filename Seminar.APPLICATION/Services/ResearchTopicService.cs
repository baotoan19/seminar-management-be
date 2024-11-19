using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.HistoryResearchTopicDtos;
using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.CORE.Utils;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;
namespace Seminar.APPLICATION.Services;

public class ResearchTopicService : IResearchTopicService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    private readonly IAuthorService _authorService;
    public ResearchTopicService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmailService emailService, IAuthorService authorService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
        _authorService = authorService;
    }

    // Research Topic
    public async Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByCompetitionIdAsync(int competitionId, int reviewCommitteeId, int index, int pageSize, string nameTopicSearch, int disciplineId, int acceptedForPublicationStatus, int ReviewAcceptanceStatus)
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
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ. Vui lòng cung cấp chỉ số và kích thước trang hợp lệ.");
        }
        IQueryable<ResearchTopic> query = _unitOfWork.GetRepository<ResearchTopic>().Entities
        .Where(r => r.DeletedAt == null && r.CompetitionId == competitionId)
        .OrderByDescending(r => r.CreatedAt);

        switch (reviewCommitteeId)
        {
            case -1: // Chưa có review committee
                query = query.Where(r => r.Review_CommitteeId == null);
                break;
            case 0: // Lấy tất cả
                break; // Không thêm điều kiện where
            case 1: // Đã có review committee
                query = query.Where(r => r.Review_CommitteeId != null);
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(nameTopicSearch))
        {
            query = query.Where(r => r.NameTopic.Contains(nameTopicSearch));
        }
        if (disciplineId > 0)
        {
            query = query.Where(r => r.DisciplineId == disciplineId);
        }

        switch (acceptedForPublicationStatus)
        {
            case 0:
                query = query.Where(r => r.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Pending);
                break;
            case 1:
                query = query.Where(r => r.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Approved);
                break;
            case 2:
                query = query.Where(r => r.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Rejected);
                break;
            case 3:
                break;
        }

        switch (ReviewAcceptanceStatus)
        {
            case 0:
                query = query.Where(r => r.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Pending);
                break;
            case 1:
                query = query.Where(r => r.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Approved);
                break;
            case 2:
                query = query.Where(r => r.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Rejected);
                break;
            case 3:
                break;
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ResearchTopicVM>(new List<ResearchTopicVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        //Author Research Topic
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Author_ResearchTopics = researchTopic.Author_ResearchTopics
                .Where(ar => ar.DeletedAt == null)
                .ToList();
        }
        // History Update Research Topic
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.History_Update_ResearchTopics = researchTopic.History_Update_ResearchTopics
                .Where(h => h.DeletedAt == null)
                .ToList();
        }
        foreach (var researchTopic in resultQuery)
        {
            foreach (var history in researchTopic.History_Update_ResearchTopics.Where(h => h.DeletedAt == null))
            {

                history.Review_Forms = history.Review_Forms
                    .Where(rf => rf.DeletedAt == null)
                    .ToList();
            }
        }
        // Review Committee
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Review_Committees.Review_Board_Members = researchTopic.Review_Committees.Review_Board_Members
                .Where(rb => rb.DeletedAt == null && rb.IsStatus == true)
                .ToList();
        }

        //Acceptance
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Acceptance.Review_Acceptances = researchTopic.Acceptance.Review_Acceptances
                .Where(ra => ra.DeletedAt == null)
                .ToList();
        }
        List<ResearchTopicVM> responeItems = _mapper.Map<List<ResearchTopicVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ResearchTopicVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<ResearchTopicVM> GetResearchTopicByIdAsync(int id)
    {
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(id) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        //Author Research Topic
        researchTopic.Author_ResearchTopics = researchTopic.Author_ResearchTopics
            .Where(ar => ar.DeletedAt == null)
            .ToList();
        // History Update Research Topic
        researchTopic.History_Update_ResearchTopics = researchTopic.History_Update_ResearchTopics
            .Where(h => h.DeletedAt == null)
            .Select(h =>
            {
                h.Review_Forms = h.Review_Forms
                    .Where(rf => rf.DeletedAt == null)
                    .ToList();
                return h;
            })
            .ToList();
        // Review Committee
        researchTopic.Review_Committees.Review_Board_Members = researchTopic.Review_Committees.Review_Board_Members
            .Where(rb => rb.DeletedAt == null && rb.IsStatus == true)
            .ToList();
        // Acceptance
        researchTopic.Acceptance.Review_Acceptances = researchTopic.Acceptance.Review_Acceptances
            .Where(ra => ra.DeletedAt == null)
            .ToList();
        return _mapper.Map<ResearchTopicVM>(researchTopic);
    }
    public async Task<PaginatedList<ResearchTopicVM>> GetAllResearchTopicByAuthorIdAsync(string roleName, int index, int pageSize, string nameTopicSearch, int acceptedForPublicationStatus, int ReviewAcceptanceStatus, int competitionId)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại. Vui lòng cung cấp tác giả hợp lệ.");
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ. Vui lòng cung cấp chỉ số và kích thước trang hợp lệ.");
        }
        IQueryable<ResearchTopic> query = _unitOfWork.GetRepository<ResearchTopic>().Entities
        .Where(r => r.DeletedAt == null && r.Author_ResearchTopics.Any(ar => ar.AuthorId == author.Id))
        .OrderByDescending(r => r.CreatedAt);

        if (!string.IsNullOrEmpty(nameTopicSearch))
        {
            query = query.Where(r => r.NameTopic.Contains(nameTopicSearch));
        }

        switch (acceptedForPublicationStatus)
        {
            case 0:
                query = query.Where(r => r.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Pending);
                break;
            case 1:
                query = query.Where(r => r.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Approved);
                break;
            case 2:
                query = query.Where(r => r.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Rejected);
                break;
            case 3:
                break;
        }

        switch (ReviewAcceptanceStatus)
        {
            case 0:
                query = query.Where(r => r.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Pending);
                break;
            case 1:
                query = query.Where(r => r.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Approved);
                break;
            case 2:
                query = query.Where(r => r.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Rejected);
                break;
            case 3:
                break;
        }

        if (competitionId > 0)
        {
            query = query.Where(r => r.CompetitionId == competitionId);
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ResearchTopicVM>(new List<ResearchTopicVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        //Author Research Topic
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Author_ResearchTopics = researchTopic.Author_ResearchTopics
                .Where(ar => ar.DeletedAt == null)
                .ToList();
        }
        // History Update Research Topic
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.History_Update_ResearchTopics = researchTopic.History_Update_ResearchTopics
                .Where(h => h.DeletedAt == null)
                .ToList();
        }
        foreach (var researchTopic in resultQuery)
        {
            foreach (var history in researchTopic.History_Update_ResearchTopics.Where(h => h.DeletedAt == null))
            {
                history.Review_Forms = history.Review_Forms
                    .Where(rf => rf.DeletedAt == null)
                    .ToList();
            }
        }

        // Review Committee
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Review_Committees.Review_Board_Members = researchTopic.Review_Committees.Review_Board_Members
                .Where(rb => rb.DeletedAt == null && rb.IsStatus == true)
                .ToList();
        }
        // Acceptance
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Acceptance.Review_Acceptances = researchTopic.Acceptance.Review_Acceptances
                .Where(ra => ra.DeletedAt == null)
                .ToList();
        }
        List<ResearchTopicVM> responeItems = _mapper.Map<List<ResearchTopicVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ResearchTopicVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task<PaginatedList<ResearchTopicVM>> GetResearchTopicsForReviewAsync(int index, int pageSize, int idSearch, string nameTopicSearch, int isStatus, int competitionId)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Reviewer reviewer = await _unitOfWork.GetRepository<Reviewer>()
            .Entities
            .FirstOrDefaultAsync(x => x.AccountId == userId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người đánh giá không tồn tại. Vui lòng cung cấp người đánh giá hợp lệ.");

        DateTime now = DateTime.Now;

        IQueryable<ResearchTopic> query = _unitOfWork.GetRepository<ResearchTopic>().Entities
            .Join(_unitOfWork.GetRepository<Review_Board_Member>().Entities,
                rt => rt.Review_CommitteeId,
                rbm => rbm.ReviewCommitteeId,
                (rt, rbm) => new { ResearchTopic = rt, ReviewBoardMember = rbm })
            .Where(x =>
                x.ResearchTopic.DeletedAt == null &&
                x.ReviewBoardMember.ReviewerId == reviewer.Id)
            .Select(x => x.ResearchTopic)
            .OrderByDescending(r => r.CreatedAt);

        // Áp dụng điều kiện isStatus
        if (isStatus == 1)
        {
            query = query.Where(r => r.Review_Committees.DateStart <= now && r.Review_Committees.DateEnd >= now);
        }
        else if (isStatus == -1)
        {
            query = query.Where(r => r.Review_Committees.DateEnd < now);
        }

        // Áp dụng bộ lọc tìm kiếm theo Id và tên
        if (idSearch > 0)
        {
            query = query.Where(r => r.Id == idSearch);
        }

        if (!string.IsNullOrEmpty(nameTopicSearch))
        {
            query = query.Where(r => r.NameTopic.Contains(nameTopicSearch));
        }

        if (competitionId > 0)
        {
            query = query.Where(r => r.CompetitionId == competitionId);
        }

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ResearchTopicVM>(new List<ResearchTopicVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        //Author Research Topic
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Author_ResearchTopics = researchTopic.Author_ResearchTopics
                .Where(ar => ar.DeletedAt == null)
                .ToList();
        }
        // History Update Research Topic
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.History_Update_ResearchTopics = researchTopic.History_Update_ResearchTopics
                .Where(h => h.DeletedAt == null)
                .ToList();
        }
        foreach (var researchTopic in resultQuery)
        {
            foreach (var history in researchTopic.History_Update_ResearchTopics.Where(h => h.DeletedAt == null))
            {
                history.Review_Forms = history.Review_Forms
                    .Where(rf => rf.DeletedAt == null)
                    .ToList();
            }
        }
        // Review Committee
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Review_Committees.Review_Board_Members = researchTopic.Review_Committees.Review_Board_Members
                .Where(rb => rb.DeletedAt == null && rb.IsStatus == true)
                .ToList();
        }
        // Acceptance
        foreach (var researchTopic in resultQuery)
        {
            researchTopic.Acceptance.Review_Acceptances = researchTopic.Acceptance.Review_Acceptances
                .Where(ra => ra.DeletedAt == null)
                .ToList();
        }
        List<ResearchTopicVM> responeItems = _mapper.Map<List<ResearchTopicVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ResearchTopicVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;
    }
    public async Task CreateResearchTopicAsync(CreateResearchTopicDto createResearchTopicDto)
    {
        var strategy = _unitOfWork.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Lấy main author
                    int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
                    Author mainAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
                        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại. Vui lòng cung cấp tác giả hợp lệ.");

                    // Kiểm tra competition còn thời gian nộp đề tài không
                    Competition competition = await _unitOfWork.GetRepository<Competition>().Entities.FirstOrDefaultAsync(c => c.Id == createResearchTopicDto.CompetitionId) ??
                        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
                    if (competition.DateEndSubmit < DateTime.Now || competition.DateStart > DateTime.Now)
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Cuộc thi đã hết hạn hoặc chưa bắt đầu.");
                    }

                    // Kiểm tra author có đăng ký đề tài thành công không
                    RegistrationForm registrationForm = await _unitOfWork.GetRepository<RegistrationForm>().Entities.FirstOrDefaultAsync(r => r.AuthorId == mainAuthor.Id && r.CompetitionId == createResearchTopicDto.CompetitionId && r.IsAccepted == (int)RegistrationFormEnum.Approved) ??
                        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không đăng ký đề tài thành công cho cuộc thi này.");
                    // Kiểm tra article có tồn tại không và article có phải của author không
                    if (createResearchTopicDto.ArticleId == 0)
                    {
                        createResearchTopicDto.ArticleId = null;
                    }
                    else
                    {
                        Article? article = await _unitOfWork.GetRepository<Article>().Entities.FirstOrDefaultAsync(a => a.Id == createResearchTopicDto.ArticleId)
                            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo không tồn tại. Vui lòng cung cấp bài báo hợp lệ.");
                        Author_Article? author_Article = await _unitOfWork.GetRepository<Author_Article>().Entities.FirstOrDefaultAsync(a => a.ArticleId == createResearchTopicDto.ArticleId && a.AuthorId == mainAuthor.Id) ??
                            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bài báo của tác giả không tồn tại.");
                        if (author_Article.AuthorId != mainAuthor.Id)
                        {
                            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Bài báo không thuộc về tác giả.");
                        }
                    }
                    // Thêm đề tài
                    ResearchTopic researchTopic = _mapper.Map<ResearchTopic>(createResearchTopicDto);
                    researchTopic.DateUpLoad = DateTime.Now;
                    researchTopic.AcceptanceApprovedStatus = (int)AcceptanceApprovedStatusEnum.Pending;
                    researchTopic.ReviewAcceptanceStatus = (int)ReviewAcceptanceStatusEnum.Pending;
                    researchTopic.Review_CommitteeId = null;
                    researchTopic.ArticleId = createResearchTopicDto.ArticleId == 0 ? null : createResearchTopicDto.ArticleId;
                    await _unitOfWork.GetRepository<ResearchTopic>().InsertAsync(researchTopic);
                    await _unitOfWork.SaveChangesAsync();

                    // Thêm main author vào đề tài
                    List<Author_ResearchTopic> author_ResearchTopics = new List<Author_ResearchTopic>
                    {
                    new Author_ResearchTopic
                    {
                        AuthorId = mainAuthor.Id,
                        ResearchTopicId = researchTopic.Id,
                        RoleName = CLAIMS_VALUES.ROLE_TYPE.AUTHOR
                    }
                    };

                    // Xử lý co-authors
                    if (createResearchTopicDto.CoAuthors != null && createResearchTopicDto.CoAuthors.Count > 0)
                    {
                        // Kiểm tra email trùng lặp
                        await ValidateCoAuthorEmailsAsync(createResearchTopicDto.CoAuthors);

                        foreach (CoAuthorDto coAuthorDto in createResearchTopicDto.CoAuthors)
                        {
                            int coAuthorId = await CreateOrUpdateCoAuthorAsync(coAuthorDto, competition, researchTopic);

                            author_ResearchTopics.Add(new Author_ResearchTopic
                            {
                                AuthorId = coAuthorId,
                                ResearchTopicId = researchTopic.Id,
                                RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR
                            });
                        }
                    }

                    await _unitOfWork.GetRepository<Author_ResearchTopic>().InsertRangeAsync(author_ResearchTopics);
                    await _unitOfWork.SaveChangesAsync();

                    // Insert history article version 1
                    History_Update_ResearchTopic history_Update_ResearchTopic = new History_Update_ResearchTopic
                    {
                        ResearchTopicId = researchTopic.Id,
                        DateUpdate = DateTime.Now,
                        Summary = createResearchTopicDto.Summary,
                        NewProductFilePath = researchTopic.ProductFilePath,
                        NewReportFilePath = researchTopic.ReportFilePath
                    };
                    await _unitOfWork.GetRepository<History_Update_ResearchTopic>().InsertAsync(history_Update_ResearchTopic);
                    await _unitOfWork.SaveChangesAsync();

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
    private async Task ValidateCoAuthorEmailsAsync(IEnumerable<CoAuthorDto> coAuthors)
    {
        var processedEmails = new HashSet<string>();
        foreach (CoAuthorDto coAuthor in coAuthors)
        {
            // Kiểm tra email trùng lặp trong danh sách
            if (!processedEmails.Add(coAuthor.Email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Co-author email is duplicated!");
            }

            // Kiểm tra email có tồn tại trong hệ thống không và vai trò của tài khoản
            Account? account = await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(a => a.Email == coAuthor.Email);
            if (account != null)
            {
                if (account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.REVIEWER || account.Role.RoleName == CLAIMS_VALUES.ROLE_TYPE.ORGANIZER)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Email is already associated with system!");
                }
            }
        }
    }
    private async Task<int> CreateOrUpdateCoAuthorAsync(CoAuthorDto coAuthorDto, Competition competition, ResearchTopic researchTopic)
    {
        Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);
        int coAuthorId;

        if (existingCoAuthor == null)
        {
            // Tạo mới co-author
            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
                        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found!");

            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            Account account = new Account
            {
                Email = coAuthorDto.Email,
                Password = passwordHasher.HashPassword(new Account(), "Huit@1245"),
                RoleId = role.Id,
                IsSuspended = false,
            };
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.SaveChangesAsync();

            Author newCoAuthor = new Author
            {
                AccountId = account.Id,
                Name = coAuthorDto.Name,
                Email = account.Email,
                NumberPhone = coAuthorDto.NumberPhone,
                DateOfBirth = coAuthorDto.DateOfBirth,
                Sex = coAuthorDto.Sex
            };
            await _unitOfWork.GetRepository<Author>().InsertAsync(newCoAuthor);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendMemberAccountInfoEmail(coAuthorDto, competition.CompetitionName);
            coAuthorId = newCoAuthor.Id;
        }
        else
        {
            coAuthorId = existingCoAuthor.Id;
            Author_ResearchTopic? existingCoAuthorResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities
                .FirstOrDefaultAsync(a => a.AuthorId == coAuthorId && a.ResearchTopicId == researchTopic.Id);
            if (existingCoAuthorResearchTopic != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Co-author is existed!");
            }
        }

        return coAuthorId;
    }
    public async Task UpdateResearchTopicAsync(int researchTopicId, UpdateResearchTopicDto updateResearchTopicDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(a => a.ResearchTopicId == researchTopicId && a.AuthorId == author.Id && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author research topic not found or not an author!");
        if (researchTopic.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "The research topic has been accepted or confirmed and cannot be edited");
        }
        var strategy = _unitOfWork.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    ResearchTopic updateResearchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ??
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
                    _mapper.Map(updateResearchTopicDto, updateResearchTopic);
                    updateResearchTopic.UpdatedAt = DateTime.Now;
                    updateResearchTopic.ArticleId = updateResearchTopicDto.ArticleId == 0 ? null : updateResearchTopicDto.ArticleId;
                    if (updateResearchTopicDto.CoAuthors != null && updateResearchTopicDto.CoAuthors.Count > 0)
                    {
                        //Lấy tất cả co-authors hiện tại (bao gồm đã xoá)
                        List<Author_ResearchTopic> existingCoAuthors = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.Where(a => a.ResearchTopicId == researchTopicId && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR).ToListAsync();

                        // Lấy danh sách email từ request
                        var newCoAuthorEmails = updateResearchTopicDto.CoAuthors.Select(x => x.Email.ToLower()).ToList();

                        // Xoá những co-author không có trong request
                        var coAuthorsToDelete = existingCoAuthors.Where(x => x.DeletedAt == null &&
                        !newCoAuthorEmails.Contains((x.Author?.Email ?? string.Empty).ToLower())).ToList();
                        foreach (var coAuthor in coAuthorsToDelete)
                        {
                            coAuthor.DeletedAt = DateTime.Now;
                        }
                        List<Author_ResearchTopic> author_ResearchTopics = new List<Author_ResearchTopic>();
                        foreach (var coAuthorDto in updateResearchTopicDto.CoAuthors)
                        {
                            await ValidateCoAuthorEmailsAsync(updateResearchTopicDto.CoAuthors);
                            var activeCoAuthor = existingCoAuthors.FirstOrDefault(x => x.Author.Email?.ToLower() == coAuthorDto.Email.ToLower() && x.DeletedAt == null);
                            if (activeCoAuthor != null)
                            {
                                continue;
                            }
                            var deletedCoAuthor = existingCoAuthors.FirstOrDefault(x => x.Author.Email?.ToLower() == coAuthorDto.Email.ToLower() && x.DeletedAt != null);
                            if (deletedCoAuthor != null)
                            {
                                deletedCoAuthor.DeletedAt = null;
                                Author? authorToUpdate = await _unitOfWork.GetRepository<Author>()
                                .Entities.FirstOrDefaultAsync(a => a.Id == deletedCoAuthor.AuthorId);
                                if (authorToUpdate != null)
                                {
                                    authorToUpdate.Name = coAuthorDto.Name;
                                    authorToUpdate.NumberPhone = coAuthorDto.NumberPhone;
                                    authorToUpdate.DateOfBirth = coAuthorDto.DateOfBirth;
                                    authorToUpdate.Sex = coAuthorDto.Sex;
                                    authorToUpdate.UpdatedAt = DateTime.Now;
                                }

                                continue;
                            }
                            Author? existingCoAuthor = await _unitOfWork.GetRepository<Author>()
                            .Entities.FirstOrDefaultAsync(a => a.Email == coAuthorDto.Email);

                            Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(researchTopic.CompetitionId) ??
                                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Competition not found!");

                            int coAuthorId = existingCoAuthor?.Id ?? await CreateOrUpdateCoAuthorAsync(coAuthorDto, competition, researchTopic);

                            author_ResearchTopics.Add(new Author_ResearchTopic
                            {
                                AuthorId = coAuthorId,
                                ResearchTopicId = researchTopic.Id,
                                RoleName = CLAIMS_VALUES.ROLE_TYPE.CO_AUTHOR
                            });
                        }
                        if (author_ResearchTopics.Any())
                        {
                            await _unitOfWork.GetRepository<Author_ResearchTopic>().InsertRangeAsync(author_ResearchTopics);
                        }
                        await _unitOfWork.SaveChangesAsync();
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
    public async Task UpdateIsReviewAcceptanceAsync(UpdateIsReviewAcceptanceDto updateIsReviewAcceptanceDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Organizer organizer = await _unitOfWork.GetRepository<Organizer>().Entities.FirstOrDefaultAsync(o => o.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Organizer not found!");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(updateIsReviewAcceptanceDto.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Research topic not found!");
        if (researchTopic.Competitions.OrganizerId != organizer.Id)
        {
            throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "You are not allowed to access this research topic!");
        }
        History_Update_ResearchTopic history_Update_ResearchTopic = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().Entities
        .Where(h => h.ResearchTopicId == researchTopic.Id)
        .OrderByDescending(h => h.CreatedAt)
        .FirstOrDefaultAsync() ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "History update research topic not found!");
        researchTopic.ProductFilePath = history_Update_ResearchTopic.NewProductFilePath;
        researchTopic.ReportFilePath = history_Update_ResearchTopic.NewReportFilePath;
        researchTopic.ReviewAcceptanceStatus = updateIsReviewAcceptanceDto.ReviewAcceptanceStatus;
        researchTopic.DateStart = DateTime.Now;
        researchTopic.DateEnd = DateTime.Now.AddMonths(researchTopic.ProjectDuration ?? 0);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateDateEndResearchTopicAsync(int researchTopicId, UpdateDateEndResearchTopicDto updateDateEndResearchTopicDto)
    {
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        researchTopic.DateEnd = DateTime.Now.AddMonths(updateDateEndResearchTopicDto.Month);
        await _unitOfWork.GetRepository<ResearchTopic>().UpdateAsync(researchTopic);
        await _unitOfWork.SaveChangesAsync();
    }
    // History Research Topic
    public async Task<List<HistoryUpdateResearchTopicVM>> GetAllHistoryResearchTopicByResearchTopicIdAsync(int researchTopicId)
    {
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(researchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        List<History_Update_ResearchTopic> history_Update_ResearchTopics = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().Entities.Where(h => h.ResearchTopicId == researchTopicId && h.DeletedAt == null).ToListAsync();
        foreach (var history in history_Update_ResearchTopics)
        {
            history.Review_Forms = history.Review_Forms
                .Where(rf => rf.DeletedAt == null)
                .ToList();
        }
        List<HistoryUpdateResearchTopicVM> historyUpdateResearchTopicVMs = _mapper
        .Map<List<HistoryUpdateResearchTopicVM>>(history_Update_ResearchTopics);

        return historyUpdateResearchTopicVMs;
    }
    public async Task CreateNewVersionResearchTopicAsync(CreateHistoryResearchTopicDto createHistoryResearchTopicDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại. Vui lòng cung cấp tác giả hợp lệ.");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(createHistoryResearchTopicDto.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        if (researchTopic.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Approved || researchTopic.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Đề tài nghiên cứu đã được chấp nhận bởi hội đồng và không thể chỉnh sửa.");
        }
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(researchTopic.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        DateTime now = DateTime.Now;
        if (competition.DateEnd < now)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Cuộc thi đã hết hạn.");
        }

        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(a => a.AuthorId == author.Id && a.ResearchTopicId == createHistoryResearchTopicDto.ResearchTopicId && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không phải là người đăng ký đề tài của cuộc thi này.");
        History_Update_ResearchTopic history_Update_ResearchTopic = _mapper.Map<History_Update_ResearchTopic>(createHistoryResearchTopicDto);
        history_Update_ResearchTopic.DateUpdate = DateTime.Now;
        await _unitOfWork.GetRepository<History_Update_ResearchTopic>().InsertAsync(history_Update_ResearchTopic);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateHistoryResearchTopicAsync(int historyResearchTopicId, UpdateHistoryResearchTopicDto updateHistoryResearchTopicDto)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại. Vui lòng cung cấp tác giả hợp lệ.");
        History_Update_ResearchTopic history_Update_ResearchTopic = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().GetByIdAsync(historyResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lịch sử cập nhật đề tài nghiên cứu không tồn tại. Vui lòng cung cấp lịch sử cập nhật đề tài nghiên cứu hợp lệ.");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(history_Update_ResearchTopic.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        Review_Form review_Form = await _unitOfWork.GetRepository<Review_Form>().Entities.FirstOrDefaultAsync(rf => rf.History_Update_ResearchTopicId == historyResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Không thể cập nhật lịch sử đề tài nghiên cứu với một bản đánh giá tồn tại.");
        if (researchTopic.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Approved || researchTopic.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Đề tài nghiên cứu đã được chấp nhận bởi hội đồng và không thể chỉnh sửa.");
        }
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(researchTopic.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        DateTime now = DateTime.Now;
        if (competition.DateEnd < now)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Cuộc thi đã hết hạn.");
        }
        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(a => a.AuthorId == author.Id && a.ResearchTopicId == researchTopic.Id && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không phải là người đăng ký đề tài của cuộc thi này.");
        _mapper.Map(updateHistoryResearchTopicDto, history_Update_ResearchTopic);
        history_Update_ResearchTopic.DateUpdate = DateTime.Now;
        history_Update_ResearchTopic.UpdatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<History_Update_ResearchTopic>().UpdateAsync(history_Update_ResearchTopic);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteHistoryResearchTopicAsync(int historyResearchTopicId)
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Author author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == userId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không tồn tại. Vui lòng cung cấp tác giả hợp lệ.");
        History_Update_ResearchTopic history_Update_ResearchTopic = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().GetByIdAsync(historyResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lịch sử cập nhật đề tài nghiên cứu không tồn tại. Vui lòng cung cấp lịch sử cập nhật đề tài nghiên cứu hợp lệ.");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(history_Update_ResearchTopic.ResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        if (researchTopic.AcceptanceApprovedStatus == (int)AcceptanceApprovedStatusEnum.Approved || researchTopic.ReviewAcceptanceStatus == (int)ReviewAcceptanceStatusEnum.Approved)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Đề tài nghiên cứu đã được chấp nhận bởi hội đồng và không thể chỉnh sửa.");
        }
        Review_Form review_Form = await _unitOfWork.GetRepository<Review_Form>().Entities.FirstOrDefaultAsync(rf => rf.History_Update_ResearchTopicId == historyResearchTopicId) ??
        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Không thể xóa lịch sử đề tài nghiên cứu với một bản đánh giá tồn tại.");
        Competition competition = await _unitOfWork.GetRepository<Competition>().GetByIdAsync(researchTopic.CompetitionId) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Cuộc thi không tồn tại. Vui lòng cung cấp cuộc thi hợp lệ.");
        DateTime now = DateTime.Now;
        if (competition.DateEnd < now)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Cuộc thi đã hết hạn.");
        }
        Author_ResearchTopic author_ResearchTopic = await _unitOfWork.GetRepository<Author_ResearchTopic>().Entities.FirstOrDefaultAsync(a => a.AuthorId == author.Id && a.ResearchTopicId == researchTopic.Id && a.RoleName == CLAIMS_VALUES.ROLE_TYPE.AUTHOR) ??
        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tác giả không phải là người đăng ký đề tài của cuộc thi này.");
        history_Update_ResearchTopic.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<History_Update_ResearchTopic>().UpdateAsync(history_Update_ResearchTopic);
        await _unitOfWork.SaveChangesAsync();
    }
}