using MoneyEzBank.Repositories.Enums;

namespace MoneyEzBank.Repositories.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = default!;

        // Navigation properties
        public Account SourceAccount { get; set; } = default!;
        public Account? DestinationAccount { get; set; }
    }
}
