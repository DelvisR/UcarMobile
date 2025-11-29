using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Payments;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Payments
{
    /// <summary>
    /// EF Core configuration for <see cref="Payout"/>.
    /// </summary>
    public class TechnicianPayoutConfiguration : IEntityTypeConfiguration<Payout>
    {
        public void Configure(EntityTypeBuilder<Payout> builder)
        {
            builder.Property(x => x.TechnicianId)
                .IsRequired();

            builder.Property(x => x.AmountCents)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(8)
                .HasDefaultValue("usd")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.ProviderPayoutId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.ProviderDestination)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Relationship
            builder.HasOne(x => x.Technician)
                .WithMany(p => p.Payouts)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for common queries
            builder.HasIndex(x => x.TechnicianId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.ProviderPayoutId);
        }
    }
}
