using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Repositories.Interfaces;

namespace MoneyEzBank.Repositories.Repositories.Implements
{
    public class WebhookConfigRepository : GenericRepository<WebhookConfig>, IWebhookConfigRepository
    {
        public WebhookConfigRepository(MoneyEzBankContext context) : base(context)
        {
        }
    }
}
