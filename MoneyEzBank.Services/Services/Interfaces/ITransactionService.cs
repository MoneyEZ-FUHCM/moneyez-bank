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
        Task<BaseResultModel> DepositAsync(CreateTransactionModel model);
        Task<BaseResultModel> WithdrawAsync(CreateTransactionModel model);
        Task<BaseResultModel> TransferAsync(CreateTransactionModel model);
        Task<BaseResultModel> GetTransactionsByAccountIdAsync(PaginationParameter paginationParameter, Guid accountId);
    }
}
