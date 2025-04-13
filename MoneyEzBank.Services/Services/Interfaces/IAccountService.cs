using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Commons.Filter;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.AccountModels;

namespace MoneyEzBank.Services.Services.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResultModel> GetByIdAsync(Guid id);
        Task<BaseResultModel> GetByAccountNumberAsync(string accountNumber);
        Task<BaseResultModel> CreateAccountAdminAsync(CreateAccountModel model);
        Task<BaseResultModel> UpdateAccountAsync(UpdateAccountModel model);
        Task<BaseResultModel> GetAccountsByFilterAsync(PaginationParameter paginationParameter, AccountFilter filter);
        Task<BaseResultModel> CreateAccountUserAsync();
        Task<BaseResultModel> DeleteAccountById(Guid id);
    }
}
