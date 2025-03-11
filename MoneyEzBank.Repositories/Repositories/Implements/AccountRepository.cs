using Microsoft.EntityFrameworkCore;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Commons.Filter;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Repositories.Repositories.Implements
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly MoneyEzBankContext _context;

        public AccountRepository(MoneyEzBankContext context) : base(context)
        {
            _context = context;
        }

        public Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            return _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber && x.IsDeleted == false);
        }

        public async Task<Pagination<Account>> GetByFilterAsync(PaginationParameter paginationParameter, AccountFilter filter)
        {
            var query = _context.Accounts.AsQueryable();

            // apply filter
            query = ApplyAccountFiltering(query, filter);

            var itemCount = await query.CountAsync();
            var items = await query.Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Account>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        private IQueryable<Account> ApplyAccountFiltering(IQueryable<Account> query, AccountFilter filter)
        {
            if (filter == null) return query;

            // Apply IsDeleted filter
            query = query.Where(u => u.IsDeleted == filter.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.Trim();

                // If field is specified, search by that field only
                if (!string.IsNullOrWhiteSpace(filter.Field))
                {
                    switch (filter.Field.ToLower())
                    {
                        case "account_number":
                            query = query.Where(u => u.AccountNumber.Contains(searchTerm));
                            break;
                    }
                }
                else
                {
                    // If no field specified, search across all searchable fields
                    query = query.Where(u =>
                        u.AccountNumber.Contains(searchTerm)
                    );
                }
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var isDescending = !string.IsNullOrWhiteSpace(filter.Dir) && filter.Dir.ToLower() == "desc";

                switch (filter.SortBy.ToLower())
                {
                    case "date":
                        query = isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt);
                        break;
                    default:
                        // Default sort by Id
                        query = isDescending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id);
                        break;
                }
            }
            else
            {
                // Default sort by Id if no sort specified
                query = query.OrderBy(u => u.Id);
            }

            return query;
        }
    }
}
