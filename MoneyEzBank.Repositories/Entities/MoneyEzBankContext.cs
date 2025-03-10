using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyEzBank.Repositories.Entities.Configurations;

namespace MoneyEzBank.Repositories.Entities
{
    public class MoneyEzBankContext : DbContext
    {
        public MoneyEzBankContext(DbContextOptions<MoneyEzBankContext> options)
            : base(options)
        {
        }

        // DbSet properties for entities
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<WebhookConfig> WebhookConfigs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new WebhookConfigConfiguration());
        }
    }
}
