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
using Seminar.APPLICATION.Dtos.ReviewerDtos;
using Seminar.APPLICATION.Dtos.OrganizersDtos;
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
        private readonly IReviewerService _reviewerService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ITokenService tokenService, IAuthorService authorService, IOrganizerService organizerService, IReviewerService reviewerService, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _tokenService = tokenService;
            _authorService = authorService;
            _organizerService = organizerService;
            _reviewerService = reviewerService;
            _logger = logger;
        }

        public async Task RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            Account? existAccount = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == registerRequestDto.Email && x.DeletedAt == null && x.Status == true);
            if (existAccount != null)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "This email is already registered.");
            }

            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(x => x.RoleName == registerRequestDto.RoleName && x.DeletedAt == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "The specified role was not found. Please provide a valid role.");

            Account account = _mapper.Map<Account>(registerRequestDto);

            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            account.Password = passwordHasher.HashPassword(account, account.Password);
            account.RoleId = role.Id;
            account.Status = true;

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
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Role name is required");
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
                    case CLAIMS_VALUES.ROLE_TYPE.REVIEWER:
                        CreateReviewerDto createReviewerDto = new CreateReviewerDto()
                        {
                            AccountId = accountId,
                            Name = registerRequestDto.Name,
                            NumberPhone = registerRequestDto.NumberPhone
                        };
                        await _reviewerService.CreateReviewerAsync(createReviewerDto);
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
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Role name is invalid");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when assign role specific service");
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "An internal server error occurred. Please try again later.");
            }
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == loginRequestDto.Email) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email not found");
            //check status
            if (account.DeletedAt != null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Account does not exist");
            }
            if (account.Status == false)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "This account is not active");
            }

            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            string hashedInputPassWord = passwordHasher.HashPassword(null, loginRequestDto.Password);
            if (hashedInputPassWord != account.Password)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "Email or password is incorrect");
            }
            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(x => x.Id == account.RoleId && x.DeletedAt == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found for the account");
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
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Refresh token is required");
            }
            return await _tokenService.RefreshAccessToken(refeshTokenRequest);
        }
        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == changePasswordDto.Email && x.DeletedAt == null && x.Status == true) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email not found");
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            string hashedInputPassWord = passwordHasher.HashPassword(null, changePasswordDto.Password);
            if (hashedInputPassWord != account.Password)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Email or password is incorrect");
            }
            if (account.Password == passwordHasher.HashPassword(null, changePasswordDto.NewPassword))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "New password and old password are the same");
            }
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Confirm password and new password do not match");
            }

            account.Password = passwordHasher.HashPassword(account, changePasswordDto.NewPassword);
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account); 
            await _unitOfWork.SaveChangesAsync();
        }
    }
}