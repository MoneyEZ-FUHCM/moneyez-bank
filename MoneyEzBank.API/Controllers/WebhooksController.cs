using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Services.BusinessModels.WebhookModels;
using MoneyEzBank.Services.Services.Interfaces;

namespace MoneyEzBank.API.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    public class WebhooksController : BaseController
    {
        private readonly IWebhookService _webhookService;

        public WebhooksController(IWebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        [HttpPost]
        public Task<IActionResult> Register([FromBody] WebhookRequestModel model)
        {
            return ValidateAndExecute(() => _webhookService.RegisterWebhookAsync(model));
        }

        [HttpPost("validate")]
        public Task<IActionResult> Validate([FromBody] ValidateBankAccountRequestModel model)
        {
            return ValidateAndExecute(() => _webhookService.ValidateBankAccountWebhook(model));
        }

        [HttpDelete("cancel/{secret}")]
        public Task<IActionResult> Cancel(string secret)
        {
            return ValidateAndExecute(() => _webhookService.CancelWebhookAsync(secret));
        }

        [HttpGet("{id}")]
        [Authorize]
        public Task<IActionResult> GetById(Guid id)
        {
            return ValidateAndExecute(() => _webhookService.GetWebhookByIdAsync(id));
        }

        [HttpPut("{id}")]
        [Authorize]
        public Task<IActionResult> Update(Guid id, [FromBody] WebhookRequestModel model)
        {
            return ValidateAndExecute(() => _webhookService.UpdateWebhookAsync(id, model));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public Task<IActionResult> Delete(Guid id)
        {
            return ValidateAndExecute(() => _webhookService.DeleteWebhookAsync(id));
        }

        [HttpGet("account/{accountId}")]
        [Authorize]
        public Task<IActionResult> GetByAccountId(Guid accountId, [FromQuery] PaginationParameter paginationParameter)
        {
            return ValidateAndExecute(() => _webhookService.GetWebhooksByAccountIdAsync(accountId, paginationParameter));
        }
    }
}
