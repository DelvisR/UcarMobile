using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Payments;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Payments;

/// <summary>
/// Entity Framework configuration for the <see cref="PaymentRefund"/> entity.
/// Defines table schema, relationships, and field constraints.
/// </summary>
public class PaymentRefundConfiguration : IEntityTypeConfiguration<PaymentRefund>
{
    public void Configure(EntityTypeBuilder<PaymentRefund> builder)
    {
        // Table name (optional - uncomment if you want explicit naming)
        // builder.ToTable("PaymentRefund");

        builder.Property(x => x.ProviderRefundId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(8)
            .HasDefaultValue("usd")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.AmountCents)
            .IsRequired();

        // Relationship: each refund belongs to a single payment
        builder.HasOne(x => x.Payment)
            .WithMany(p => p.Refunds) // ensure Payment has ICollection<PaymentRefund> Refunds
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.ProviderRefundId).IsUnique();
    }
}
