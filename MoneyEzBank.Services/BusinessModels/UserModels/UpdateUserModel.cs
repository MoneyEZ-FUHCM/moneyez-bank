using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.BusinessModels.UserModels
{
    public class UpdateUserModel : CreateUserModel
    {
        public Guid Id { get; set; }
    }
}
