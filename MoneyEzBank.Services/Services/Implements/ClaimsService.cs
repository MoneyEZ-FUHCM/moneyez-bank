using Microsoft.AspNetCore.Http;
using MoneyEzBank.Services.Services.Interfaces;
using MoneyEzBank.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Services.Implements
{
    public class ClaimsService : IClaimsService
    {
        public ClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            // todo implementation to get the current userId
            var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var extractedId = ClaimsUtils.GetEmailFromIdentity(identity);
            GetCurrentUserEmail = extractedId;
        }

        public string GetCurrentUserEmail { get; }
    }
}
