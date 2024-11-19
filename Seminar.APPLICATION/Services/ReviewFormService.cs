using AutoMapper;
using Castle.Core.Logging;
using FirebaseAdmin.Auth.Multitenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.ReviewFormDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.APPLICATION.Services;

public class ReviewFormService : IReviewFormService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    public ReviewFormService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
    }
    public async Task<PaginatedList<ReviewFormVM>> GetAllReviewFormByHistoryUpdateResearchTopicIdAsync(int historyUpdateResearchTopicId, int index, int pageSize)
    {
        History_Update_ResearchTopic history_Update_ResearchTopic = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().GetByIdAsync(historyUpdateResearchTopicId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "History Update Research Topic not found!");
        if (index <= 0 || pageSize <= 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Invalid index or page size");
        }
        IQueryable<Review_Form> query = _unitOfWork.GetRepository<Review_Form>().Entities
        .Where(x => x.History_Update_ResearchTopicId == historyUpdateResearchTopicId && x.DeletedAt == null)
        .OrderByDescending(x => x.Date_Upload);

        int totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ReviewFormVM>(new List<ReviewFormVM>(), 0, index, pageSize);
        }
        var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        List<ReviewFormVM> responeItems = _mapper.Map<List<ReviewFormVM>>(resultQuery);
        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var responePaginatedList = new PaginatedList<ReviewFormVM>(
            responeItems,
            totalCount,
            index,
            pageSize
        );
        return responePaginatedList;

    }
    private async Task<int> GetReviewerId()
    {
        int userId = int.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
        Reviewer reviewer = await _unitOfWork.GetRepository<Reviewer>().Entities.FirstOrDefaultAsync(x => x.AccountId == userId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người đánh giá không tồn tại. Vui lòng cung cấp người đánh giá hợp lệ.");
        return reviewer.Id;
    }
    public async Task CreateReviewFormAsync(CreateReviewFormDto createReviewFormDto)
    {
        int ReviewerId = await GetReviewerId();
        History_Update_ResearchTopic history_Update_ResearchTopic =
            await _unitOfWork.GetRepository<History_Update_ResearchTopic>().GetByIdAsync(createReviewFormDto.History_Update_ResearchTopicId) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND,
                "Lịch sử cập nhật đề tài nghiên cứu không tồn tại. Vui lòng cung cấp lịch sử cập nhật đề tài nghiên cứu hợp lệ.");
        ResearchTopic researchTopic =
            await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(history_Update_ResearchTopic.ResearchTopicId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND,
                "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        Review_Board_Member review_Board_Member =
            await _unitOfWork.GetRepository<Review_Board_Member>().Entities
                .FirstOrDefaultAsync(x => x.ReviewerId == ReviewerId && x.ReviewCommitteeId == researchTopic.Review_CommitteeId)
            ?? throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN,
                "Bạn không phải là thành viên của hội đồng này!");
        Review_Committee review_Committee =
            await _unitOfWork.GetRepository<Review_Committee>().GetByIdAsync(researchTopic.Review_CommitteeId ?? 0)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND,
            "Hội đồng phản biện không tồn tại!");
        DateTime now = DateTime.Now;
        if (now < review_Committee.DateStart)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA,
            "Hội đồng phản biện chưa bắt đầu!");
        }
        if (now > review_Committee.DateEnd)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA,
                "Hội đồng phản biện đã kết thúc!");
        }
        Review_Form? review_Form = await _unitOfWork.GetRepository<Review_Form>().Entities
            .FirstOrDefaultAsync(x => x.ReviewerId == ReviewerId &&
                                x.History_Update_ResearchTopicId == history_Update_ResearchTopic.Id &&
                                x.DeletedAt == null);
        if (review_Form != null)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA,
            "Bạn đã đánh giá bản này của đề tài nghiên cứu!");
        }
        // Create new review form
        review_Form = _mapper.Map<Review_Form>(createReviewFormDto);
        review_Form.ReviewerId = ReviewerId;
        review_Form.Date_Upload = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Form>().InsertAsync(review_Form);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateReviewFormAsync(int id, UpdateReviewFormDto updateReviewFormDto)
    {
        int ReviewerId = await GetReviewerId();
        Review_Form review_Form = await _unitOfWork.GetRepository<Review_Form>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bản đánh giá không tồn tại. Vui lòng cung cấp bản đánh giá hợp lệ.");
        if (review_Form.ReviewerId != ReviewerId) throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không phải là người đánh giá của bản này!");
        History_Update_ResearchTopic history_Update_ResearchTopic = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().GetByIdAsync(review_Form.History_Update_ResearchTopicId ?? 0) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lịch sử cập nhật đề tài nghiên cứu không tồn tại. Vui lòng cung cấp lịch sử cập nhật đề tài nghiên cứu hợp lệ.");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(history_Update_ResearchTopic.ResearchTopicId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        Review_Committee review_Committee = await _unitOfWork.GetRepository<Review_Committee>().GetByIdAsync(researchTopic.Review_CommitteeId ?? 0) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Hội đồng phản biện không tồn tại. Vui lòng cung cấp hội đồng phản biện hợp lệ.");
        DateTime now = DateTime.Now;
        if (now < review_Committee.DateStart)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Hội đồng phản biện chưa bắt đầu!");
        }
        if (now > review_Committee.DateEnd)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Hội đồng phản biện đã kết thúc!");
        }
        review_Form = _mapper.Map<Review_Form>(updateReviewFormDto);
        review_Form.UpdatedAt = DateTime.Now;
        review_Form.Date_Upload = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Form>().UpdateAsync(review_Form);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteReviewFormAsync(int id)
    {
        int ReviewerId = await GetReviewerId();
        Review_Form review_Form = await _unitOfWork.GetRepository<Review_Form>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bản đánh giá không tồn tại. Vui lòng cung cấp bản đánh giá hợp lệ.");
        if (review_Form.ReviewerId != ReviewerId) throw new ErrorException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Bạn không phải là người đánh giá của bản này!");
        History_Update_ResearchTopic history_Update_ResearchTopic = await _unitOfWork.GetRepository<History_Update_ResearchTopic>().GetByIdAsync(review_Form.History_Update_ResearchTopicId ?? 0) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lịch sử cập nhật đề tài nghiên cứu không tồn tại. Vui lòng cung cấp lịch sử cập nhật đề tài nghiên cứu hợp lệ.");
        ResearchTopic researchTopic = await _unitOfWork.GetRepository<ResearchTopic>().GetByIdAsync(history_Update_ResearchTopic.ResearchTopicId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đề tài nghiên cứu không tồn tại. Vui lòng cung cấp đề tài nghiên cứu hợp lệ.");
        Review_Committee review_Committee = await _unitOfWork.GetRepository<Review_Committee>().GetByIdAsync(researchTopic.Review_CommitteeId ?? 0) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Review Committee not found!");
        DateTime now = DateTime.Now;
        if (now < review_Committee.DateStart)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Hội đồng phản biện chưa bắt đầu!");
        }
        if (now > review_Committee.DateEnd)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Hội đồng phản biện đã kết thúc!");
        }
        review_Form.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Review_Form>().UpdateAsync(review_Form);
        await _unitOfWork.SaveChangesAsync();
    }
}
