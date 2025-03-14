using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MoneyEzBank.Repositories.Enums;

namespace MoneyEzBank.Repositories.Entities
{
    public class WebhookConfig : BaseEntity
    {
        [Required]
        [Url]
        public string Url { get; set; } = default!;

        [Required]
        public WebhookType Type { get; set; }

        [Required]
        [MinLength(64)]
        public string Secret { get; set; } = default!;

        // Audit fields
        public DateTime? LastTriggeredAt { get; set; }
        public DateTime? LastFailureAt { get; set; }
        public int FailureCount { get; set; }

        // Configuration fields
        [Required]
        public bool IsEnabled { get; set; } = true;
        public int MaxRetries { get; set; } = 3;
        public int RetryIntervalSeconds { get; set; } = 60;
        public string ContentType { get; set; } = "application/json";

        // Optional account-specific webhook
        public Guid AccountId { get; set; }
        public virtual Account? Account { get; set; }
    }
}
