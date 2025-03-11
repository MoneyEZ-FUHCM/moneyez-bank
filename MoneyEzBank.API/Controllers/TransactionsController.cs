using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Services.BusinessModels.TransactionModels;
using MoneyEzBank.Services.Services.Interfaces;

namespace MoneyEzBank.API.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        [Authorize]
        public Task<IActionResult> Deposit(CreateTransactionModel model)
        {
            return ValidateAndExecute(() => _transactionService.DepositAsync(model));
        }

        [HttpPost("withdraw")]
        [Authorize]
        public Task<IActionResult> Withdraw(CreateTransactionModel model)
        {
            return ValidateAndExecute(() => _transactionService.WithdrawAsync(model));
        }

        [HttpPost("transfer")]
        [Authorize]
        public Task<IActionResult> Transfer(CreateTransactionModel model)
        {
            return ValidateAndExecute(() => _transactionService.TransferAsync(model));
        }

        [HttpGet("account/{accountId}")]
        [Authorize]
        public Task<IActionResult> GetTransactionsByAccountId([FromQuery] PaginationParameter paginationParameter, Guid accountId)
        {
            return ValidateAndExecute(() => _transactionService.GetTransactionsByAccountIdAsync(paginationParameter, accountId));
        }
    }
}
