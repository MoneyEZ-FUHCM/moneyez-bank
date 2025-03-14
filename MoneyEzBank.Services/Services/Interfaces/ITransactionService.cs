using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.TransactionModels;

namespace MoneyEzBank.Services.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<BaseResultModel> DepositAsync(CreateDepositModel model);
        Task<BaseResultModel> WithdrawAsync(CreateWithdrawModel model);
        Task<BaseResultModel> TransferAsync(CreateTransferModel model);
        Task<BaseResultModel> GetTransactionsByAccountIdAsync(PaginationParameter paginationParameter, Guid accountId);
    }
}
