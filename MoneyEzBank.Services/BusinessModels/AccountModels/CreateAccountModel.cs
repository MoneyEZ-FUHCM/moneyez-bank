namespace MoneyEzBank.Services.BusinessModels.AccountModels
{
    public class CreateAccountModel
    {
        public string AccountNumber { get; set; } = default!;
        public decimal InitialBalance { get; set; }
        public Guid UserId { get; set; }
    }
}
