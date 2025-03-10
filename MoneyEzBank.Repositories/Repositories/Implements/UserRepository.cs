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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly MoneyEzBankContext _context;

        public UserRepository(MoneyEzBankContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.PhoneNumber == phoneNumber);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<List<User>> GetUsersByUserIdsAsync(List<Guid> userIds)
        {
            return await _context.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<Pagination<User>> GetUsersByFilter(PaginationParameter paginationParameter, UserFilter filter)
        {
            var query = _context.Users.AsQueryable();

            // apply filter
            query = ApplyUserFiltering(query, filter);

            var itemCount = await query.CountAsync();
            var items = await query.Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<User>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        private IQueryable<User> ApplyUserFiltering(IQueryable<User> query, UserFilter filter)
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
                        case "email":
                            query = query.Where(u => u.Email.Contains(searchTerm));
                            break;
                        case "fullname":
                            query = query.Where(u => u.FullName.Contains(searchTerm) || u.UnsignFullName.Contains(searchTerm));
                            break;
                        case "phone":
                            query = query.Where(u => u.PhoneNumber.Contains(searchTerm));
                            break;
                    }
                }
                else
                {
                    // If no field specified, search across all searchable fields
                    query = query.Where(u =>
                        u.Email.Contains(searchTerm) ||
                        u.FullName.Contains(searchTerm) ||
                        u.UnsignFullName.Contains(searchTerm) ||
                        u.PhoneNumber.Contains(searchTerm)
                    );
                }
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var isDescending = !string.IsNullOrWhiteSpace(filter.Dir) && filter.Dir.ToLower() == "desc";

                switch (filter.SortBy.ToLower())
                {
                    case "email":
                        query = isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                        break;
                    case "fullname":
                        query = isDescending ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName);
                        break;
                    case "phone":
                        query = isDescending ? query.OrderByDescending(u => u.PhoneNumber) : query.OrderBy(u => u.PhoneNumber);
                        break;
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
