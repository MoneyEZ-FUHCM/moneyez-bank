using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyEzBank.Services.BusinessModels.AuthenModels;
using MoneyEzBank.Services.Services.Interfaces;

namespace MoneyEzBank.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;

        public AuthController(IUserService userService, IClaimsService claimsService)
        {
            _userService = userService;
            _claimsService = claimsService;
        }

        [HttpPost("login")]
        public Task<IActionResult> LoginWithEmailPassword(LoginModel loginModel)
        {
            return ValidateAndExecute(() => _userService.LoginWithEmailPassword(loginModel.Email, loginModel.Password));
        }

        [HttpPost("register")]
        public Task<IActionResult> Register(SignUpModel signUpModel)
        {
            return ValidateAndExecute(() => _userService.RegisterAsync(signUpModel));
        }
    }
}
