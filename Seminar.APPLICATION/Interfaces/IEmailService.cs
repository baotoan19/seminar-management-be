using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.AuthorDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IEmailService
{
    Task SendReviewerAccountInfoEmail(RegisterRequestDto request);
    Task SendCoAuthorAccountInfoEmail(CoAuthorDto coAuthorDto);
    Task SendMemberAccountInfoEmail(CoAuthorDto coAuthorDto, string competitionName);
}
