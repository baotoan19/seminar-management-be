using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.DOMAIN.Interfaces;
using AutoMapper;
using Seminar.DOMAIN.Entitys;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Seminar.CORE.ExceptionCustom;
using Microsoft.AspNetCore.Http;
using Seminar.CORE.Constants;
using Seminar.CORE.Utils;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Seminar.APPLICATION.Interfaces.IOrganizerService;
using Microsoft.Extensions.Logging;
using Seminar.CORE.Base;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Dtos.OtpDtos;
using Seminar.DOMAIN.Enum;
namespace Seminar.APPLICATION.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IAuthorService _authorService;
        private readonly IOrganizerService _organizerService;
        private readonly IOtpService _otpService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ITokenService tokenService, IAuthorService authorService, IOrganizerService organizerService, IOtpService otpService, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _tokenService = tokenService;
            _authorService = authorService;
            _organizerService = organizerService;
            _otpService = otpService;
            _logger = logger;
        }

        public async Task SendRegistrationOtpAsync(string email)
        {
            // Kiểm tra email đã tồn tại chưa
            Account? existAccount = await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(x => x.Email == email && x.DeletedAt == null && x.IsSuspended == false);
            if (existAccount != null)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable,
                    ResponseCodeConstants.EXISTED, "Email đã được đăng ký.");
            }

            // Gửi OTP
            await _otpService.GenerateOtp(new OtpRequestDto()
            {
                Email = email,
                OtpType = OtpTypeEnum.Registration,
            });
        }
        public async Task RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            // Verify OTP trước
            bool isOtpVerified = await _otpService.VerifyOtp(new OtpVerificationDto()
            {
                Email = registerRequestDto.Email,
                OtpCode = registerRequestDto.OtpCode,
            });
            if (!isOtpVerified)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "Mã OTP không chính xác");
            }
            
            // Kiểm tra role
            Role role = await _unitOfWork.GetRepository<Role>().Entities
                .FirstOrDefaultAsync(x => x.RoleName == registerRequestDto.RoleName &&x.DeletedAt == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    "Vai trò không tồn tại. Vui lòng cung cấp vai trò hợp lệ.");

            // Tạo tài khoản
            Account account = _mapper.Map<Account>(registerRequestDto);
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(
                Options.Create(new PasswordHasherOptions()));
            account.Password = passwordHasher.HashPassword(account, account.Password);
            account.RoleId = role.Id;
            account.IsSuspended = false;

            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.SaveChangesAsync();

            await AssignRoleSpecificService(account.Id, registerRequestDto);
        }
        //Kiểm tra role và gọi service tương ứng
        private async Task AssignRoleSpecificService(int accountId, RegisterRequestDto registerRequestDto)
        {
            try
            {
                string roleName = registerRequestDto.RoleName;
                if (roleName == string.Empty)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vai trò là bắt buộc");
                }
                switch (roleName)
                {
                    case CLAIMS_VALUES.ROLE_TYPE.AUTHOR:
                        CreateAuthorDto createAuthorDto = new CreateAuthorDto()
                        {
                            AccountId = accountId,
                            Name = registerRequestDto.Name,
                            Email = registerRequestDto.Email,
                            NumberPhone = registerRequestDto.NumberPhone,
                        };
                        await _authorService.CreateAuthorAsync(createAuthorDto);
                        break;
                    case CLAIMS_VALUES.ROLE_TYPE.ORGANIZER:
                        CreateOrganizerDto createOrganizerDto = new CreateOrganizerDto()
                        {
                            AccountId = accountId,
                            Name = registerRequestDto.Name,
                            NumberPhone = registerRequestDto.NumberPhone,
                        };
                        await _organizerService.CreateOrganizerAsync(createOrganizerDto);
                        break;
                    default:
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Tên vai trò không hợp lệ");
                }
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã xảy ra lỗi máy chủ nội bộ. Vui lòng thử lại sau.");
            }
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == loginRequestDto.Email) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email không tồn tại");
            //check status
            if (account.DeletedAt != null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tài khoản không tồn tại");
            }
            if (account.IsSuspended == true)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "Tài khoản không hoạt động");
            }

            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            string hashedInputPassWord = passwordHasher.HashPassword(null, loginRequestDto.Password);
            if (hashedInputPassWord != account.Password)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "Email hoặc mật khẩu không chính xác");
            }
            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(x => x.Id == account.RoleId && x.DeletedAt == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Vai trò không tồn tại cho tài khoản");
            string roleName = role.RoleName;
            TokenResponseDto tokenResponseDto = _tokenService.GenerateToken(account, roleName);
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                TokenResponse = tokenResponseDto,
            };
            return loginResponseDto;
        }
        public async Task<TokenResponseDto> RefreshAccessTokenAsync(RefeshTokenRequestDto refeshTokenRequest)
        {
            if (string.IsNullOrEmpty(refeshTokenRequest.RefreshToken))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Refresh token là bắt buộc");
            }
            return await _tokenService.RefreshAccessToken(refeshTokenRequest);
        }
        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == changePasswordDto.Email && x.DeletedAt == null && x.IsSuspended == false) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email không tồn tại");
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            string hashedInputPassWord = passwordHasher.HashPassword(null, changePasswordDto.Password);
            if (hashedInputPassWord != account.Password)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Email hoặc mật khẩu không chính xác");
            }
            if (account.Password == passwordHasher.HashPassword(null, changePasswordDto.NewPassword))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Mật khẩu mới và mật khẩu cũ giống nhau");
            }
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Mật khẩu xác nhận và mật khẩu mới không khớp");
            }

            account.Password = passwordHasher.HashPassword(account, changePasswordDto.NewPassword);
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}