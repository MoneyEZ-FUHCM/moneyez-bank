using MoneyEzBank.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Commons.Filter;

namespace MoneyEzBank.Repositories.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task<Pagination<Account>> GetByFilterAsync(PaginationParameter paginationParameter, AccountFilter filter);
    }
}
