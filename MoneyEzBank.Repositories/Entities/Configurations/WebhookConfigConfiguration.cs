using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoneyEzBank.Repositories.Entities.Configurations
{
    public class WebhookConfigConfiguration : IEntityTypeConfiguration<WebhookConfig>
    {
        public void Configure(EntityTypeBuilder<WebhookConfig> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(w => w.Url)
                .IsRequired();

            builder.Property(w => w.Secret)
                .IsRequired();
        }
    }
}
