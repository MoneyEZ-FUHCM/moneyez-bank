using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Services.Interfaces
{
    public interface IClaimsService
    {
        public string GetCurrentUserEmail { get; }
    }
}
