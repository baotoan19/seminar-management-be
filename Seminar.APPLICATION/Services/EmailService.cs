using System.Net;
using System.Net.Mail;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class EmailService : IEmailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    public EmailService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task SendReviewerAccountInfoEmail(RegisterRequestDto request)
    { 
        var emailBody = await CreateEmailBodySendAccountInfoAsync(request);
        var sender = _configuration["EmailSettings:Sender"];
        var password = _configuration["EmailSettings:Password"];
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(sender),
            Subject = "Xác nhận đăng ký tài khoản phản biện",
            Body = emailBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(request.Email);

        using (var smtpClient = new SmtpClient(host,port))
        {
            smtpClient.Credentials = new NetworkCredential(sender, password);
            smtpClient.EnableSsl = true;
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã xảy ra lỗi khi gửi email");
            }
        }
    }

    private async Task<string> CreateEmailBodySendAccountInfoAsync(RegisterRequestDto request)
    {
        string UserId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        var btcEmail = await _unitOfWork.GetRepository<Organizer>().Entities.Where(x => x.AccountId == int.Parse(UserId)).Select(x => x.Account.Email).FirstOrDefaultAsync();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<body style=\"margin: 0; padding: 0; background-color: #f9f9f9; font-family: Arial, sans-serif;\">");
        sb.AppendLine("    <div style=\"width: 100%; max-width: 650px; margin: 50px auto; background-color: #ffffff; border-radius: 12px; box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);\">");
        sb.AppendLine("      ");
        sb.AppendLine("      <!-- Header -->");
        sb.AppendLine("      <div style=\"background-color: #4CAF50; padding: 30px; text-align: center; border-top-left-radius: 12px; border-top-right-radius: 12px;\">");
        sb.AppendLine("        <h1 style=\"margin: 0; font-size: 26px; font-weight: bold; color: #fff; letter-spacing: 1px; text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);\">");
        sb.AppendLine("          🎉 Rất vui được đồng hành cùng bạn trong vai trò Người Phản Biện!");
        sb.AppendLine("        </h1>");
        sb.AppendLine("      </div>");
        sb.AppendLine("");
        sb.AppendLine("      <!-- Nội dung chính -->");
        sb.AppendLine("      <div style=\"padding: 25px 35px; color: #444;\">");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px;\">");
        sb.AppendLine($"          Xin chào <strong>{request.Name}</strong>, 👋");
        sb.AppendLine("        </p>");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px;\">");
        sb.AppendLine("          Chúng tôi rất vui mừng khi bạn đã đồng ý trở thành một phần của đội ngũ phản biện. ");
        sb.AppendLine("          Vai trò của bạn sẽ giúp chúng tôi nâng cao chất lượng và giá trị của các công trình nghiên cứu khoa học.");
        sb.AppendLine("        </p>");
        sb.AppendLine("");
        sb.AppendLine("        <h2 style=\"font-size: 22px; margin-bottom: 15px; color: #333; border-bottom: 2px solid #4CAF50; display: inline-block; padding-bottom: 5px;\">");
        sb.AppendLine("          Thông tin đăng nhập của bạn");
        sb.AppendLine("        </h2>");
        sb.AppendLine("        <ul style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px; padding-left: 20px;\">");
        sb.AppendLine($"          <li><strong>Tên đăng nhập:</strong> {request.Email}</li>");
        sb.AppendLine($"          <li><strong>Mật khẩu:</strong> {request.Password}</li>");
        sb.AppendLine("        </ul>");
        sb.AppendLine("");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px;\">");
        sb.AppendLine("          Vì lý do bảo mật, chúng tôi khuyến nghị bạn <strong>đổi mật khẩu ngay sau khi đăng nhập lần đầu</strong>. ");
        sb.AppendLine("        </p>");
        sb.AppendLine("");
        sb.AppendLine("        <!-- Nút hành động -->");
        sb.AppendLine("        <a href=\"{{Link đăng nhập}}\" style=\"display: inline-block; padding: 14px 28px; background-color: #4CAF50; color: white; text-decoration: none; font-size: 18px; border-radius: 8px; margin-top: 10px; font-weight: bold;\">");
        sb.AppendLine("          Đăng nhập ngay");
        sb.AppendLine("        </a>");
        sb.AppendLine("");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-top: 25px;\">");
        sb.AppendLine("          Nếu gặp vấn đề khi đăng nhập, đừng ngần ngại liên hệ với chúng tôi. ");
        sb.AppendLine("          Chúng tôi luôn sẵn sàng hỗ trợ bạn! 💬");
        sb.AppendLine("        </p>");
        sb.AppendLine("      </div>");
        sb.AppendLine("");
        sb.AppendLine("      <!-- Footer -->");
        sb.AppendLine("      <div style=\"text-align: center; padding: 15px; background-color: #f0f0f0; border-bottom-left-radius: 12px; border-bottom-right-radius: 12px; font-size: 14px; color: #777;\">");
        sb.AppendLine("        <p>");
        sb.AppendLine("          📧 Liên hệ: <a href=\"mailto:support@example.com\" style=\"color: #4CAF50; text-decoration: none;\">");
        sb.AppendLine($" {btcEmail}");
        sb.AppendLine("          </a>");
        sb.AppendLine("        </p>");
        sb.AppendLine($"        <p>📅 Ngày gửi: {DateTime.Now.ToString("dd/MM/yyyy")}</p>");
        sb.AppendLine("        <p>&copy; 2024 Ban Tổ Chức. Mọi quyền được bảo lưu.</p>");
        sb.AppendLine("      </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </body>");
        return sb.ToString();
    }

}