using Microsoft.EntityFrameworkCore.Storage;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Repositories.Implements;
using MoneyEzBank.Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MoneyEzBankContext _context;
        private IDbContextTransaction _transaction;

        private IUserRepository _userRepository;

        public UnitOfWork(MoneyEzBankContext context)
        {
            _context = context;
        }

        public IUserRepository UsersRepository
        {
            get
            {
                return _userRepository ??= new UserRepository(_context);

            }
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction?.Commit();
            }
            catch (Exception)
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
