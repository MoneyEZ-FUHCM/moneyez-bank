using MoneyEzBank.Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UsersRepository { get; }
        IAccountRepository AccountsRepository { get; }
        ITransactionRepository TransactionsRepository { get; }
        int Save();
        void Commit();
        void Rollback();
        Task SaveAsync();
    }
}
