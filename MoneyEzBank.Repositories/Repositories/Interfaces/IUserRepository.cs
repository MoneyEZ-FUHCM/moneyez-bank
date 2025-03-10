using MoneyEzBank.Repositories.Commons.Filter;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Repositories.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByPhoneAsync(string phoneNumber);

        Task<List<User>> GetUsersByUserIdsAsync(List<Guid> userIds);

        Task<Pagination<User>> GetUsersByFilter(PaginationParameter paginationParameter, UserFilter filter);
    }
}
