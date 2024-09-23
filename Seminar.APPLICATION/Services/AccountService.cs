using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AccountDtos;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        private async Task<Account> GetAccountById(int id)
        {
            return await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Account not found");
        }

        public async Task<AccountVM> GetAccountByIdAsync(int id)
        {
            Account account = await GetAccountById(id);
            var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(account.RoleId ?? 0)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found");

            var accountVM = _mapper.Map<AccountVM>(account);
            accountVM.RoleName = role.RoleName;
            return accountVM;
        }

        public async Task UpdateAccountAsync(int id, UpdateAccountDto updateAccountDto)
        {
            Account account = await GetAccountById(id);
            _mapper.Map(updateAccountDto, account);
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(int id)
        {
            Account account = await GetAccountById(id);
            account.DeletedAt = DateTime.Now;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeAccountId = null)
        {
            return await _unitOfWork.GetRepository<Account>().Entities.AnyAsync(x => x.Email == email && x.Id != excludeAccountId);
        }
    }
}
