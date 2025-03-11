namespace MoneyEzBank.Services.BusinessModels.AccountModels
{
    public class UpdateAccountModel
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = default!;
        public Guid UserId { get; set; }
    }
}
