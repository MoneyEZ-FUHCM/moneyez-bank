using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoneyEzBank.Repositories.Entities.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.AccountNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.AccountHolder).IsRequired().HasMaxLength(250);

            builder.Property(a => a.Balance)
                .HasPrecision(18, 2);

            builder.HasIndex(a => a.AccountNumber)
                .IsUnique();

            builder.HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
