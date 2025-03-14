using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.BusinessModels.TransactionModels
{
    public class CreateWithdrawModel
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
