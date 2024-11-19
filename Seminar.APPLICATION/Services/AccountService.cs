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
using Seminar.INFRASTRUCTURE.Common;

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

        public async Task<PaginatedList<AccountVM>> GetPagedAsync(int index, int pageSize, string idSearch, string nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Chỉ số hoặc kích thước trang không hợp lệ!");
            }

            IQueryable<Account> query = _unitOfWork.GetRepository<Account>().Entities
                .Include(a => a.Role)
                .Where(a => a.DeletedAt == null)
                .OrderByDescending(a => a.CreatedAt);

            //Tìm kiếm theo id
            if (!string.IsNullOrEmpty(idSearch))
            {
                query = query.Where(a => a.Id.ToString().Contains(idSearch));
                bool isInt = int.TryParse(idSearch, out int idInt);
                if (!isInt)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tài khoản không tồn tại!");
                }
            }

            //Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(nameSearch))
            {
                query = query.Where(a => EF.Functions.Like(a.Email, $"%{nameSearch}%"));
                var result = await query.ToListAsync();
                if (result.Count == 0)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tài khoản không tồn tại!");
                }
            }

            int totalCount = await query.CountAsync();
            if (totalCount == 0)
            {
                return new PaginatedList<AccountVM>(new List<AccountVM>(), 0, index, pageSize);
            }
            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            List<AccountVM> responeItems = _mapper.Map<List<AccountVM>>(resultQuery);
            var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
            var responePaginatedList = new PaginatedList<AccountVM>(
                responeItems,
                totalCount,
                index,
                pageSize
            );
            return responePaginatedList;
        }
        private async Task<Account> GetAccountById(int id)
        {
            return await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tài khoản không tồn tại!");
        }
        public async Task<AccountVM> GetAccountByIdAsync(int id)
        {
            Account account = await GetAccountById(id);
            var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(account.RoleId ?? 0)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role không tồn tại!");

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

        public async Task<string> GetNameByAccountId(int accountId)
        {
            var author = await _unitOfWork.GetRepository<Author>().Entities
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (author != null) return author.Name ?? string.Empty;

            var reviewer = await _unitOfWork.GetRepository<Reviewer>().Entities
                .FirstOrDefaultAsync(r => r.AccountId == accountId);
            if (reviewer != null) return reviewer.Name ?? string.Empty;

            var organizer = await _unitOfWork.GetRepository<Organizer>().Entities
                .FirstOrDefaultAsync(o => o.AccountId == accountId);
            if (organizer != null) return organizer.Name ?? string.Empty;

            return "Super Admin";
        }

        public async Task<string> GetEmailByAccountId(int accountId)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(a => a.Id == accountId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tài khoản không tồn tại!");
            return account.Email ?? string.Empty;
        }
    }
}
