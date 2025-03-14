using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.WebhookModels;

namespace MoneyEzBank.Services.Services.Interfaces
{
    public interface IWebhookService
    {
        Task NotifyBalanceChangeAsync(WebhookPayload payload);
        Task<BaseResultModel> RegisterWebhookAsync(WebhookRequestModel model);
        Task<BaseResultModel> GetWebhookByIdAsync(Guid id);
        Task<BaseResultModel> UpdateWebhookAsync(Guid id, WebhookRequestModel model);
        Task<BaseResultModel> DeleteWebhookAsync(Guid id);
        Task<BaseResultModel> GetWebhooksByAccountIdAsync(Guid accountId, PaginationParameter paginationParameter);
    }
}
