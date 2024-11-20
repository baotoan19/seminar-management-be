using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.EmailDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IEmailService
{
    Task SendReviewerAccountInfoEmail(ReviewBoardMemberDto request, string reviewCommitteeName);
    Task SendCoAuthorAccountInfoEmail(CoAuthorDto coAuthorDto);
    Task SendMemberAccountInfoEmail(CoAuthorDto coAuthorDto, string competitionName);
    Task SendOtpEmail(string email, string otpCode);
    Task SendFeedBackEmail(EmailFeedBackDto emailFeedBackDto);
}
