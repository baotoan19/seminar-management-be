using Seminar.APPLICATION.Dtos.AuthDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IEmailService
{
    Task SendReviewerAccountInfoEmail(RegisterRequestDto request);
}