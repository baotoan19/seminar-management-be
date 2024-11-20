using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.EmailDtos;
using Seminar.APPLICATION.Dtos.ReviewCommitteeDtos;
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

    private async Task SendEmailAsync(string recipientEmail, string subject, string body)
    {
        var sender = _configuration["EmailSettings:Sender"];
        var password = _configuration["EmailSettings:Password"];
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(sender),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(recipientEmail);

        using (var smtpClient = new SmtpClient(host, port))
        {
            smtpClient.Credentials = new NetworkCredential(sender, password);
            smtpClient.EnableSsl = true;
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the exception details here
                throw new ErrorException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    "Đã xảy ra lỗi khi gửi email");
            }
        }
    }
    public async Task SendReviewerAccountInfoEmail(ReviewBoardMemberDto request, string reviewCommitteeName)
    {
        var emailBody = await CreateEmailBodySendAccountReviewerAsync(request, reviewCommitteeName);
        await SendEmailAsync(request.Email, "Xác nhận đăng ký tài khoản phản biện", emailBody);
    }

    public async Task SendCoAuthorAccountInfoEmail(CoAuthorDto coAuthorDto)
    {
        var emailBody = await CreateEmailBodySendAccountCoAuthorAsync(coAuthorDto);
        await SendEmailAsync(coAuthorDto.Email, "Xác nhận đăng ký tài khoản đồng tác giả", emailBody);
    }

    public async Task SendMemberAccountInfoEmail(CoAuthorDto coAuthorDto, string competitionName)
    {
        var emailBody = await CreateEmailBodySendAccountMemberAsync(coAuthorDto, competitionName);
        await SendEmailAsync(coAuthorDto.Email, "Xác nhận đăng ký tài khoản thành viên", emailBody);
    }

    public async Task SendOtpEmail(string email, string otpCode)
    {
        var emailBody = await CreateEmailBodySendOtpAsync(otpCode);
        await SendEmailAsync(email, "Mã OTP", emailBody);
    }

    public async Task SendFeedBackEmail(EmailFeedBackDto emailFeedBackDto)
    {
        var emailBody = await CreateEmailBodySendFeedBackAsync(emailFeedBackDto);
        await SendEmailAsync(emailFeedBackDto.EmailReviewer, "Phản biện đề tài", emailBody);
    }

    private async Task<string> CreateEmailBodySendAccountReviewerAsync(ReviewBoardMemberDto request, string reviewCommitteeName)
    {
        string UserId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        string loginUrl = "http://localhost:3000/login";
        var btcEmail = await _unitOfWork.GetRepository<Organizer>().Entities
                        .Where(x => x.AccountId == int.Parse(UserId))
                        .Select(x => x.Account.Email)
                        .FirstOrDefaultAsync();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<body style=\"margin: 0; padding: 0; background-color: #f9f9f9; font-family: Arial, sans-serif;\">");
        sb.AppendLine("    <div style=\"width: 100%; max-width: 650px; margin: 50px auto; background-color: #ffffff; border-radius: 12px; box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);\">");
        sb.AppendLine("");
        sb.AppendLine("      <div style=\"background-color: #4CAF50; padding: 30px; text-align: center; border-top-left-radius: 12px; border-top-right-radius: 12px;\">");
        sb.AppendLine("        <h1 style=\"margin: 0; font-size: 26px; font-weight: bold; color: #fff; letter-spacing: 1px; text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);\">");
        sb.AppendLine($"          🎉 Chào mừng bạn đến với {reviewCommitteeName} - nơi bạn sẽ đóng vai trò quan trọng với tư cách là một Người Phản Biện xuất sắc!");
        sb.AppendLine("        </h1>");
        sb.AppendLine("      </div>");
        sb.AppendLine("");
        sb.AppendLine("      <!-- Main Content -->");
        sb.AppendLine("      <div style=\"padding: 25px 35px; color: #444;\">");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px;\">");
        sb.AppendLine($"          Xin chào <strong>{request.Name}</strong>, 👋");
        sb.AppendLine("        </p>");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px;\">");
        sb.AppendLine($"          Chúng tôi rất vinh dự được bạn gia nhập vào hội đồng {reviewCommitteeName}. Vai trò của bạn là chìa khóa giúp nâng cao chất lượng và giá trị của các công trình nghiên cứu khoa học.");
        sb.AppendLine("        </p>");
        sb.AppendLine("");
        sb.AppendLine("        <h2 style=\"font-size: 22px; margin-bottom: 15px; color: #333; border-bottom: 2px solid #4CAF50; display: inline-block; padding-bottom: 5px;\">");
        sb.AppendLine("          Thông tin đăng nhập của bạn");
        sb.AppendLine("        </h2>");
        sb.AppendLine("        <ul style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px; padding-left: 20px;\">");
        sb.AppendLine($"          <li><strong>Tên đăng nhập:</strong> {request.Email}</li>");
        sb.AppendLine($"          <li><strong>Mật khẩu:</strong> Huit@1245</li>");
        sb.AppendLine("        </ul>");
        sb.AppendLine("");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-bottom: 20px;\">");
        sb.AppendLine("          Vì lý do bảo mật, chúng tôi khuyến nghị bạn <strong>đổi mật khẩu ngay sau khi đăng nhập lần đầu</strong>.");
        sb.AppendLine("        </p>");
        sb.AppendLine("");
        sb.AppendLine("        <!-- Action Button -->");
        sb.AppendLine("        <a href=\"" + loginUrl + "\" style=\"display: inline-block; padding: 14px 28px; background-color: #4CAF50; color: white; text-decoration: none; font-size: 18px; border-radius: 8px; margin-top: 10px; font-weight: bold;\">");
        sb.AppendLine("          Đăng nhập ngay");
        sb.AppendLine("        </a>");
        sb.AppendLine("");
        sb.AppendLine("        <p style=\"font-size: 17px; line-height: 1.8; margin-top: 25px;\">");
        sb.AppendLine("          Nếu bạn gặp bất kỳ vấn đề nào khi đăng nhập, đừng ngần ngại liên hệ với chúng tôi. Chúng tôi luôn sẵn sàng hỗ trợ bạn! 💬");
        sb.AppendLine("        </p>");
        sb.AppendLine("      </div>");
        sb.AppendLine("");
        sb.AppendLine("      <!-- Footer -->");
        sb.AppendLine("      <div style=\"text-align: center; padding: 15px; background-color: #f0f0f0; border-bottom-left-radius: 12px; border-bottom-right-radius: 12px; font-size: 14px; color: #777;\">");
        sb.AppendLine("        <p>");
        sb.AppendLine("          📧 Liên hệ: <a href=\"mailto:support@example.com\" style=\"color: #4CAF50; text-decoration: none;\">");
        sb.AppendLine($"            {btcEmail}");
        sb.AppendLine("          </a>");
        sb.AppendLine("        </p>");
        sb.AppendLine($"        <p>📅 Ngày gửi: {DateTime.Now.ToString("dd/MM/yyyy")}</p>");
        sb.AppendLine("        <p>&copy; 2024 Ban Tổ Chức. Mọi quyền được bảo lưu.</p>");
        sb.AppendLine("      </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </body>");
        return sb.ToString();
    }

    private async Task<string> CreateEmailBodySendAccountCoAuthorAsync(CoAuthorDto coAuthorDto)
    {
        string loginUrl = "http://localhost:3000/login";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(@"
    <body style=""margin: 0; padding: 0; background-color: #f9f9f9; font-family: Arial, sans-serif;"">
        <div style=""width: 100%; max-width: 720px; margin: 50px auto; background-color: #ffffff; 
                    border-radius: 12px; box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);"">
        
        <!-- Header -->
        <div style=""background-color: #2196F3; padding: 30px; text-align: center; 
                    border-top-left-radius: 12px; border-top-right-radius: 12px;"">
            <h1 style=""margin: 0; font-size: 26px; font-weight: bold; color: #fff; letter-spacing: 1px;
                        text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);"">
            🎉 Chào mừng bạn đến với vai trò Đồng Tác Giả!
            </h1>
        </div>

        <!-- Nội dung chính -->
        <div style=""padding: 25px 35px; color: #444;"">
            <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
            Xin chào <strong>" + coAuthorDto.Name + @"</strong>, 👋
            </p>
            <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
            Chúng tôi vô cùng vui mừng khi bạn đã đồng ý trở thành một phần của đội ngũ đồng tác giả. 
            Sự hợp tác của bạn sẽ giúp chúng tôi tạo ra những nghiên cứu chất lượng cao và có giá trị, 
            góp phần vào sự phát triển của cộng đồng khoa học.
            </p>

            <h2 style=""font-size: 22px; margin-bottom: 15px; color: #333; border-bottom: 2px solid #2196F3;
                        display: inline-block; padding-bottom: 5px;"">
            Thông tin tài khoản của bạn
            </h2>
            <div style=""background-color: #e3f2fd; padding: 15px; border-radius: 8px;"">
            <ul style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px; padding-left: 20px;"">
                <li><strong>Tên đăng nhập:</strong> " + coAuthorDto.Email + @"</li>
                <li><strong>Mật khẩu:</strong> " + "Huit@1245" + @"</li>
            </ul>
            </div>

            <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
            Để bảo mật tài khoản của bạn, chúng tôi khuyên bạn nên <strong>đổi mật khẩu ngay khi đăng nhập lần đầu</strong>. 
            Điều này sẽ giúp bảo vệ thông tin của bạn và đảm bảo rằng bạn có trải nghiệm tốt nhất trong quá trình làm việc với chúng tôi.
            </p>

            <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
            Khi bạn đăng nhập vào hệ thống, hãy truy cập vào phần hồ sơ của mình để xem lại những bài báo mà bạn đã đóng góp. 
            Đây là nơi bạn có thể theo dõi tiến trình nghiên cứu của mình và thấy rõ sự ảnh hưởng của bạn trong cộng đồng. 
            Chúng tôi tin rằng mỗi đóng góp của bạn đều mang một giá trị độc đáo và đáng tự hào.
            </p>

            <!-- Nút hành động -->
            <a href=""" + loginUrl + @""" style=""display: inline-block; padding: 14px 28px; 
                    background-color: #2196F3; color: white; text-decoration: none; font-size: 18px; 
                    border-radius: 8px; margin-top: 10px; font-weight: bold;"">
            Đăng nhập ngay
            </a>

            <p style=""font-size: 17px; line-height: 1.8; margin-top: 25px;"">
            Nếu bạn gặp bất kỳ khó khăn nào trong quá trình đăng nhập, hãy liên hệ với chúng tôi. 
            Chúng tôi luôn sẵn sàng hỗ trợ bạn để bạn có thể bắt đầu công việc của mình một cách suôn sẻ! 💬
            </p>
        </div>

        <!-- Footer -->
        <div style=""text-align: center; padding: 15px; background-color: #f0f0f0; 
                    border-bottom-left-radius: 12px; border-bottom-right-radius: 12px; font-size: 14px; color: #777;"">
            <p>
            📧 Liên hệ: <a href=""mailto:admin@gmail.com"" style=""color: #2196F3; text-decoration: none;"">
                admin@gmail.com
            </a>
            </p>
            <p>📅 Ngày gửi: " + DateTime.Now.ToString("dd/MM/yyyy") + @"</p>
            <p>&copy; 2024 Ban Tổ Chức. Mọi quyền được bảo lưu.</p>
        </div>
        </div>
    </body>
        ");
        return sb.ToString();
    }

    private async Task<string> CreateEmailBodySendAccountMemberAsync(CoAuthorDto coAuthorDto, string competitionName)
    {
        string loginUrl = "http://localhost:3000/login";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(@"
        <body style=""margin: 0; padding: 0; background-color: #f9f9f9; font-family: Arial, sans-serif;"">
            <div style=""width: 100%; max-width: 720px; margin: 50px auto; background-color: #ffffff; 
                        border-radius: 12px; box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);"">
            
            <!-- Header -->
            <div style=""background-color: #2196F3; padding: 30px; text-align: center; 
                        border-top-left-radius: 12px; border-top-right-radius: 12px;"">
                <h1 style=""margin: 0; font-size: 26px; font-weight: bold; color: #fff; letter-spacing: 1px;
                            text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);"">
                🎓 Chào mừng bạn đến với vai trò Thành Viên!
                </h1>
            </div>

            <!-- Nội dung chính -->
            <div style=""padding: 25px 35px; color: #444;"">
                <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
                Xin chào <strong>" + coAuthorDto.Name + @"</strong>, 👋
                </p>
                <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
                Chúng tôi rất vui khi bạn đã trở thành thành viên của <strong>“" + competitionName + @"”</strong>. 
                Sự đóng góp của bạn sẽ giúp chúng ta hoàn thành những mục tiêu đã đề ra và tạo ra những giá trị đáng kể cho cộng đồng nghiên cứu khoa học.
                </p>

                <h2 style=""font-size: 22px; margin-bottom: 15px; color: #333; border-bottom: 2px solid #2196F3;
                            display: inline-block; padding-bottom: 5px;"">
                Thông tin tài khoản của bạn
                </h2>
                <div style=""background-color: #e3f2fd; padding: 15px; border-radius: 8px;"">
                <ul style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px; padding-left: 20px;"">
                    <li><strong>Tên đăng nhập:</strong> " + coAuthorDto.Email + @"</li>
                    <li><strong>Mật khẩu:</strong> " + "Huit@1245" + @"</li>
                </ul>
                </div>

                <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
                Để bảo mật tài khoản của bạn, chúng tôi khuyên bạn nên <strong>đổi mật khẩu ngay khi đăng nhập lần đầu</strong>. 
                Điều này sẽ giúp bảo vệ thông tin của bạn và đảm bảo rằng bạn có trải nghiệm tốt nhất trong quá trình làm việc với chúng tôi.
                </p>

                <p style=""font-size: 17px; line-height: 1.8; margin-bottom: 20px;"">
                Sau khi đăng nhập vào hệ thống, bạn có thể truy cập vào phần hồ sơ của mình để xem lại các đề tài nghiên cứu đã tham gia. 
                Tại đây, bạn có thể theo dõi tiến độ của các đề tài và nhận ra được sự đóng góp của mình trong lĩnh vực khoa học. 
                Chúng tôi tin rằng mỗi đóng góp của bạn đều mang lại một giá trị riêng biệt và đáng tự hào.
                </p>

                <!-- Nút hành động -->
                <a href=""" + loginUrl + @""" style=""display: inline-block; padding: 14px 28px; 
                        background-color: #2196F3; color: white; text-decoration: none; font-size: 18px; 
                        border-radius: 8px; margin-top: 10px; font-weight: bold;"">
                Đăng nhập ngay
                </a>

                <p style=""font-size: 17px; line-height: 1.8; margin-top: 25px;"">
                Nếu bạn gặp bất kỳ khó khăn nào trong quá trình đăng nhập, hãy liên hệ với chúng tôi. 
                Chúng tôi luôn sẵn sàng hỗ trợ bạn để bạn có thể bắt đầu công việc của mình một cách suôn sẻ! 💬
                </p>
            </div>

            <!-- Footer -->
            <div style=""text-align: center; padding: 15px; background-color: #f0f0f0; 
                        border-bottom-left-radius: 12px; border-bottom-right-radius: 12px; font-size: 14px; color: #777;"">
                <p>
                📧 Liên hệ: <a href=""mailto:admin@gmail.com"" style=""color: #2196F3; text-decoration: none;"">
                    admin@gmail.com
                </a>
                </p>
                <p>📅 Ngày gửi: " + DateTime.Now.ToString("dd/MM/yyyy") + @"</p>
                <p>&copy; 2024 Ban Tổ Chức. Mọi quyền được bảo lưu.</p>
            </div>
            </div>
        </body>
        ");
        return sb.ToString();
    }

    private async Task<string> CreateEmailBodySendOtpAsync(string otpCode)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(@"<!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Xác nhận OTP</title>
        </head>
        <body style=""margin: 0; padding: 0; background-color: #f3f7fa; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;"">
            <div style=""width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); border-radius: 10px; box-sizing: border-box;"">
                <!-- Header -->
                <div style=""text-align: center; padding: 20px 0; background-color: #ff9800; color: #ffffff; border-radius: 8px 8px 0 0; position: relative;"">
                    <img src=""https://img.icons8.com/ios-filled/50/ffffff/email-open.png"" alt=""Email Icon"" style=""width: 40px; height: 40px; position: absolute; top: 10px; left: 20px;"">
                    <h1 style=""margin: 0; font-size: 24px;"">Xác nhận Email</h1>
                </div>

                <!-- Content -->
                <div style=""padding: 20px; text-align: center;"">
                    <p style=""font-size: 16px; color: #333; line-height: 1.6;"">
                        Kính chào bạn,<br>
                        Chúng tôi đã nhận được yêu cầu xác thực email của bạn. Vui lòng nhập mã OTP bên dưới để hoàn tất việc xác minh:
                    </p>
                    
                    <!-- OTP Box -->
                    <div style=""background-color: #fff7e6; padding: 20px; margin: 20px 0; border-radius: 8px; border: 2px dashed #ff9800;"">
                        <h2 style=""margin: 0; color: #ff9800; font-size: 32px; letter-spacing: 4px;"">
                            " + otpCode + @"
                        </h2>
                    </div>

                    <p style=""font-size: 14px; color: #666; margin-top: 20px;"">
                        Mã OTP này sẽ hết hạn sau 2 phút.<br>
                        Vui lòng không chia sẻ mã với bất kỳ ai để bảo vệ tài khoản của bạn.
                    </p>

                    <!-- Warning -->
                    <div style=""margin-top: 30px; padding: 15px; background-color: #fff3cd; border-radius: 8px; border: 1px solid #ffeeba; text-align: left;"">
                        <p style=""margin: 0; color: #856404; font-size: 14px;"">
                            <strong>Lưu ý:</strong> Nếu bạn không yêu cầu xác thực này, hãy bỏ qua email hoặc liên hệ với bộ phận hỗ trợ của chúng tôi.
                        </p>
                    </div>
                </div>

                <!-- Footer -->
                <div style=""margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; text-align: center; color: #999;"">
                    <p style=""font-size: 13px; margin: 0;"">
                        © 2024 Hệ thống quản lý hội thảo. Bảo lưu mọi quyền.<br>
                        Đây là email tự động, vui lòng không trả lời.
                    </p>
                </div>
            </div>
        </body>
        </html>");
        return sb.ToString();
    }

    private async Task<string> CreateEmailBodySendFeedBackAsync(EmailFeedBackDto emailFeedBackDto)
    {
        string emailBody = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Reminder Email</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    line-height: 1.8;
                    color: #3a3a3a;
                    background-color: #f4f7f9;
                    margin: 0;
                    padding: 20px;
                    font-size: 16px; /* Đặt cỡ chữ mặc định */
                }}
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #ffffff;
                    border-radius: 10px;
                    overflow: hidden;
                    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
                border: 2px solid rgba(169, 169, 169, 0.6);
                }}
                .header {{
                    background: linear-gradient(90deg, #4CAF50, #1976D2);
                    color: white;
                    text-align: center;
                    padding: 30px 20px;
                    ont-size: 24px; /* Điều chỉnh cỡ chữ tiêu đề */
                }}
                .header h2 {{
                    margin: 0;
                    font-size: 26px;
                    font-weight: bold;
                }}
                .content {{
                    padding: 25px;
                }}
                .info-section {{
                    background-color: #f9f9f9;
                    padding: 20px;
                    border-radius: 8px;
                    margin: 20px 0;
                    border: 1px solid #e4e4e4;
                }}
                .info-section ul {{
                    list-style: none;
                    padding: 0;
                    margin: 0;
                }}
                .info-section li {{
                    padding: 10px 0;
                    border-bottom: 1px solid #ddd;
                    font-size: 15px;
                }}
                .info-section li:last-child {{
                    border-bottom: none;
                }}
                .message {{
                    background-color: #e8f5e9;
                    padding: 20px;
                    border-left: 6px solid #4CAF50;
                    margin: 20px 0;
                    font-size: 16px;
                    line-height: 1.6;
                    border-radius: 5px;
                }}
                .footer {{
                    text-align: center;
                    padding: 15px 20px;
                    background-color: #f4f4f4;
                    color: #6c757d;
                    font-size: 13px;
                    border-top: 1px solid #e0e0e0;
                }}
                .signature {{
                    margin-top: 30px;
                    padding-top: 20px;
                    font-style: italic;
                    border-top: 1px solid #e6e6e6;
                }}
                .highlight {{
                    color: #1976D2;
                    font-weight: bold;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>Đề xuất hoàn thành phản biện</h2>
                </div>
                <div class='content'>
                    <p>Kính gửi <strong>{emailFeedBackDto.NameReviewer}</strong>,</p>
                    <p>Tôi là <strong>{emailFeedBackDto.NameAuthor}</strong>, tác giả của đề tài &quot;<span class='highlight'>{emailFeedBackDto.TitleResearchTopic}</span>&quot;. Tôi xin phép gửi lời nhắc nhở nhẹ nhàng về tiến độ phản biện. Rất mong nhận được phản hồi từ quý thầy/cô/anh/chị trong thời gian sớm nhất để tôi có thể chỉnh sửa và hoàn thiện đúng hạn.</p>

                    <div class='info-section'>
                        <ul>
                            <li><strong>Đề tài:</strong> {emailFeedBackDto.TitleResearchTopic}</li>
                            <li><strong>Người gửi:</strong> {emailFeedBackDto.NameAuthor}</li>
                            <li><strong>Ngày gửi:</strong> {DateTime.Now.ToString("dd/MM/yyyy")}</li>
                            <li><strong>Ngày đề xuất:</strong> {emailFeedBackDto.ProposedTime.ToString("dd/MM/yyyy")}</li>
                        </ul>
                    </div>

                    <p><strong>Lời nhắn từ tác giả:</strong></p>
                    <div class='message'>
                        {emailFeedBackDto.Feedback}
                    </div>

                    <div class='signature'>
                        <p>Xin chân thành cảm ơn quý thầy/cô/anh/chị vì sự hỗ trợ quý giá này.</p>
                        <p><strong>{HttpUtility.HtmlEncode(emailFeedBackDto.NameAuthor ?? "Không có thông tin")}</strong></p>
                        <p><strong>Thông tin liên hệ:</strong></p>
                        <p>Email: {HttpUtility.HtmlEncode(emailFeedBackDto.EmailAuthor ?? "Không có thông tin")}</p>
                        <p>Điện thoại: {HttpUtility.HtmlEncode(emailFeedBackDto.PhoneAuthor ?? "Không có thông tin")}</p>
                    </div>
                </div>
            <div class='footer'>
                <p>Email này được gửi tự động từ hệ thống. Vui lòng không trả lời trực tiếp email này.</p>
                <p>&copy; 2024 Hệ thống quản lý phản biện. Mọi quyền được bảo lưu.</p>
            </div>
        </div>
    </body>
        </html>";
        return emailBody;
    }
}