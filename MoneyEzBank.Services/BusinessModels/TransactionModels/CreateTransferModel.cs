using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.BusinessModels.TransactionModels
{
    public class CreateTransferModel
    {
        public Guid SourceAccountId { get; set; }
        [Required(ErrorMessage = "Destination bank is require")]
        public string DestinationBank { get; set; } = "";
        public string DestinationAccountNumber { get; set; } = "";
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
