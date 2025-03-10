using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyEzBank.Services.Services.Interfaces;

namespace MoneyEzBank.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;

        public UsersController(IUserService userService, IClaimsService claimsService)
        {
            _userService = userService;
            _claimsService = claimsService;
        }

        [HttpGet("current")]
        [Authorize]
        public Task<IActionResult> GetCurrentUser()
        {
            string currentEmail = _claimsService.GetCurrentUserEmail;
            return ValidateAndExecute(() => _userService.GetCurrentUser(currentEmail));
        }
    }
}
