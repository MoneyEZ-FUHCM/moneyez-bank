using System.ComponentModel.DataAnnotations;

namespace MoneyEzBank.Services.BusinessModels.AccountModels
{
    public class CreateAccountModel
    {
        [Required]
        [StringLength(12)]
        public string AccountNumber { get; set; } = default!;
        public decimal InitialBalance { get; set; } = 0;
        public Guid UserId { get; set; }
    }
}
