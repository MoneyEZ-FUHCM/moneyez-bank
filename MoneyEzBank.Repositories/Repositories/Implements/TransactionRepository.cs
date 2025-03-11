using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Repositories.Repositories.Implements
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(MoneyEzBankContext context) : base(context)
        {
        }
    }
}
