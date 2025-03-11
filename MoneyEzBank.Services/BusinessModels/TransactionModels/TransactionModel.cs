using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyEzBank.Repositories.Enums;

namespace MoneyEzBank.Services.BusinessModels.TransactionModels
{
    public class TransactionModel
    {
        public Guid Id { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
