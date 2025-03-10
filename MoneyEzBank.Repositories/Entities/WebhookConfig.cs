using MoneyEzBank.Repositories.Enums;

namespace MoneyEzBank.Repositories.Entities
{
    public class WebhookConfig : BaseEntity
    {
        public string Url { get; set; } = default!;
        public WebhookType Type { get; set; }
        public string Secret { get; set; } = default!;
        public DateTime? LastTriggeredAt { get; set; }
    }
}
