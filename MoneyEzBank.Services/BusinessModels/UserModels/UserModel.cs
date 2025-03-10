using MoneyEzBank.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.BusinessModels.UserModels
{
    public class UserModel : BaseEntity
    {
        public string? Email { get; set; } = "";

        public string? PasswordHash { get; set; } = "";

        public string? FullName { get; set; } = "";

        public string? PhoneNumber { get; set; }
    }
}
