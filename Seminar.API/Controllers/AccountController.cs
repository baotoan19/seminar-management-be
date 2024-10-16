using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seminar.APPLICATION.Dtos.AccountDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Base;
using Seminar.CORE.Constants;
using Seminar.INFRASTRUCTURE.Common;

namespace Seminar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.SUPPERADMIN)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetPagedAsync(int index = 1, int pageSize = 8, string idSearch = "", string nameSearch = "")
        {
            var accounts = await _accountService.GetPagedAsync(index, pageSize, idSearch, nameSearch);
            return Ok(new BaseResponse<PaginatedList<AccountVM>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: accounts));
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            return Ok(new BaseResponse<AccountVM>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: account));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAccount(int id, UpdateAccountDto updateAccountDto)
        {
            await _accountService.UpdateAccountAsync(id, updateAccountDto);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Update account success"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            await _accountService.DeleteAccountAsync(id);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Delete account success"));
        }
    }
}