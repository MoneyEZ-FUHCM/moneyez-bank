namespace MoneyEzBank.Services.BusinessModels.AccountModels
{
    public class AccountModel
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = default!;
        public decimal Balance { get; set; }
        public Guid UserId { get; set; }
    }
}
