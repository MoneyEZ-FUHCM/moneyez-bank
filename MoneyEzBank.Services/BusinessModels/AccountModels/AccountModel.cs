using MoneyEzBank.Repositories.Entities;

namespace MoneyEzBank.Services.BusinessModels.AccountModels
{
    public class AccountModel : BaseEntity
    {
        public string AccountNumber { get; set; } = default!;
        public string? AccountHolder { get; set; }
        public decimal Balance { get; set; }
        public Guid UserId { get; set; }
    }
}
