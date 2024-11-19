using Microsoft.EntityFrameworkCore.Storage;

namespace Seminar.APPLICATION.Models;

public class StatisticsVM
{
    public CompetitionStatistics CompetitionStatistics { get; set; }
    public RegistrationFormStatistics RegistrationFormStatistics { get; set; }
    public AuthorStatistics AuthorStatistics { get; set; }
    public ReviewCommitteeStatistics ReviewCommitteeStatistics { get; set; }
    public ArticleStatistics ArticleStatistics { get; set; }
    public DisciplineStatistics DisciplineStatistics { get; set; }
    public ResearchFieldStatistics ResearchFieldStatistics { get; set; }
}

public class CompetitionStatistics
{
    public int TotalCompetition { get; set; } // Tổng số cuộc thi
    public List<CompetitionVM> Competition { get; set; } // Chi tiết cuộc thi
    public int UpcomingCompetition { get; set; } // Số cuộc thi sắp diễn ra
    public int OngoingCompetition { get; set; } // Số cuộc thi đang diễn ra
    public int FinishedCompetition { get; set; } // Số cuộc thi đã kết thúc
}

public class RegistrationFormStatistics
{
    public int TotalRegistrationForm { get; set; } // Tổng số phiếu đăng ký
    public int ApprovedRegistrationForm { get; set; }     // Số phiếu đã được duyệt
    public int PendingRegistrationForm { get; set; }      // Số phiếu chưa được duyệt
    public int RejectedRegistrationForm { get; set; }      // Số phiếu đã bị từ chối
    public double SuccessfulRegistrationRate { get; set; } // Tỷ lệ đăng ký thành công
}

public class AuthorStatistics
{
    public int TotalAuthor { get; set; } // Tổng số tác giả
    public int TotalCoAuthor { get; set; } // Tổng số tác giả đồng tác giả
}

public class ReviewCommitteeStatistics
{
    public int TotalReviewCommittee { get; set; } // Tổng số thành viên hội đồng
    public int TotalReviewer { get; set; } // Tổng số thành viên phản biện
    public double ReviewerParticipationRate { get; set; } // Tỷ lệ tham gia phản biện
}

public class ArticleStatistics
{
    public int TotalArticle { get; set; } // Tổng số bài báo
}

public class DisciplineStatistics
{
    public int TotalDiscipline { get; set; } // Tổng số lĩnh vực
    public List<DisciplineDetailStatistics> DisciplineDetailStatistics { get; set; } // Chi tiết thống kê lĩnh vực
}

public class DisciplineDetailStatistics
{
    public DisciplineVM Discipline { get; set; } // Lĩnh vực
    public int Count { get; set; } // Số lượng
    public double Percent { get; set; } // Tỷ lệ
}

public class ResearchFieldStatistics
{
    public int TotalResearchTopic { get; set; }                // Tổng số chủ đề nghiên cứu
    public int TotalSuccessfulReviewedTopics { get; set; }     // Tổng số đề tài nghiên cứu được phản biện thành công
    public int TotalPendingReviewTopics { get; set; }         // Tổng số đề tài nghiên cứu chưa được phản biện
    public double SuccessfulReviewRate { get; set; }           // Tỉ lệ số đề tài phản biện thành công
    public int TotalRejectedReviewTopics { get; set; }         // Tổng số đề tài bị từ chối phản biện
    public int TotalFacultyPendingReviewTopics { get; set; } // Tổng số đề tài nghiên cứu chưa được khoa phản biện
    public int TotalFacultyApprovedTopics { get; set; }        // Tổng số đề tài được khoa phê duyệt
    public int TotalFacultyRejectedTopics { get; set; }        // Tổng số đề tài được khoa từ chối
    public int TotalPublishedTopics { get; set; }              // Tổng số đề tài được public lên hệ thống
    public int TotalPendingPublishedTopics { get; set; }      // Tổng số đề tài chưa được public lên hệ thống
    public int TotalRejectedPublishedTopics { get; set; }     // Tổng số đề tài không được public lên hệ thống
    public double PublishedTopicsRate { get; set; }            // Tỷ lệ đề tài được public lên hệ thống
}

