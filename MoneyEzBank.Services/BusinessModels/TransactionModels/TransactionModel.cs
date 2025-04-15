using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Enums;

namespace MoneyEzBank.Services.BusinessModels.TransactionModels
{
    public class TransactionModel :  BaseEntity
    {
        public Guid SourceAccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = default!;
        public decimal TransactionDirection { get; set; } // New field to indicate positive or negative direction
    }
}
