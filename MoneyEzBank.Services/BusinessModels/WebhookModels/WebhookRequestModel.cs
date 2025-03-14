using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MoneyEzBank.Repositories.Enums;

namespace MoneyEzBank.Services.BusinessModels.WebhookModels
{
    public class WebhookRequestModel
    {
        [Required(ErrorMessage = "URL is required")]
        [Url(ErrorMessage = "A valid URL is required")]
        public required string Url { get; set; }

        [Required(ErrorMessage = "Secret is required")]
        [MinLength(32, ErrorMessage = "Secret must be at least 32 characters long")]
        [MaxLength(256, ErrorMessage = "Secret cannot exceed 256 characters")]
        public required string Secret { get; set; }

        [Required(ErrorMessage = "Account number is required")]
        [StringLength(20, ErrorMessage = "Account number cannot exceed 20 characters")]
        public required string AccountNumber { get; set; }
    }
}
