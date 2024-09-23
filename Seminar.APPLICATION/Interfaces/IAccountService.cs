using Seminar.APPLICATION.Dtos.AccountDtos;
using Seminar.APPLICATION.Dtos.AuthDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces
{
    public interface IAccountService
    {
        Task<AccountVM> GetAccountByIdAsync(int id);
        Task UpdateAccountAsync(int id, UpdateAccountDto updateAccountDto);
        Task DeleteAccountAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeAccountId = null);
    }
}
