using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Commons.Filter;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.AuthenModels;
using MoneyEzBank.Services.BusinessModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Services.Interfaces
{
    public interface IUserService
    {
        public Task<BaseResultModel> RegisterAsync(SignUpModel model);

        public Task<BaseResultModel> LoginWithEmailPassword(string email, string password);

        public Task<BaseResultModel> RefreshToken(string jwtToken);

        //public Task<BaseResultModel> ChangePasswordAsync(string email, ChangePasswordModel changePasswordModel);

        // manager user

        public Task<BaseResultModel> GetUserByIdAsync(Guid id);

        public Task<BaseResultModel> GetUserPaginationAsync(PaginationParameter paginationParameter, UserFilter userFilter);

        public Task<BaseResultModel> CreateUserAsync(CreateUserModel model);

        public Task<BaseResultModel> UpdateUserAsync(UpdateUserModel model);

        public Task<BaseResultModel> DeleteUserAsync(Guid id, string currentEmail);

        public Task<BaseResultModel> BanUserAsync(Guid id, string currentEmail);

        public Task<BaseResultModel> GetCurrentUser(string email);
    }
}
