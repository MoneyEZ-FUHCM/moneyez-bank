using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Commons.Filter;
using MoneyEzBank.Services.BusinessModels.AccountModels;
using MoneyEzBank.Services.Services.Interfaces;

namespace MoneyEzBank.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize]
        public Task<IActionResult> GetAccounts([FromQuery] PaginationParameter paginationParameter, [FromQuery] AccountFilter filter)
        {
            return ValidateAndExecute(() => _accountService.GetAccountsByFilterAsync(paginationParameter, filter));
        }

        [HttpGet("{id}")]
        [Authorize]
        public Task<IActionResult> GetAccountById(Guid id)
        {
            return ValidateAndExecute(() => _accountService.GetByIdAsync(id));
        }

        [HttpGet("number/{accountNumber}")]
        [Authorize]
        public Task<IActionResult> GetByAccountNumber(string accountNumber)
        {
            return ValidateAndExecute(() => _accountService.GetByAccountNumberAsync(accountNumber));
        }

        [HttpPost]
        [Authorize]
        public Task<IActionResult> CreateAccount()
        {
            return ValidateAndExecute(() => _accountService.CreateAccountUserAsync());
        }

        [HttpPost("admin")]
        [Authorize(Roles = "ADMIN")]
        public Task<IActionResult> CreateAccountAdmin(CreateAccountModel createAccountModel)
        {
            return ValidateAndExecute(() => _accountService.CreateAccountAdminAsync(createAccountModel));
        }

        [HttpPut]
        [Authorize]
        public Task<IActionResult> UpdateAccount(UpdateAccountModel model)
        {
            return ValidateAndExecute(() => _accountService.UpdateAccountAsync(model));
        }

        [HttpDelete("admin")]
        [Authorize(Roles = "ADMIN")]
        public Task<IActionResult> DeleteAccountById(Guid id)
        {
            return ValidateAndExecute(() => _accountService.DeleteAccountById(id));
        }
    }
}
