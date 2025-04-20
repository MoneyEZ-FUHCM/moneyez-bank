using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.BusinessModels.WebhookModels
{
    public class WebhookConfigModel : BaseEntity
    {
        public string? Url { get; set; }

        public string? Type { get; set; }

        public string? Secret { get; set; }

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
        public string? AccountNumber { get; set; }
        public string? AccountHolder { get; set; }
    }
}
