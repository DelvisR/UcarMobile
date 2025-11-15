using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Payments;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Payments;

/// <summary>
/// Entity Framework configuration for the <see cref="Payment"/> entity.
/// Defines schema, constraints, and relationships.
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Optional table naming
        // builder.ToTable("Payment");

        builder.Property(x => x.ProviderPaymentId)
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

        builder.Property(x => x.ClientSecret)
            .HasMaxLength(100);

        builder.Property(x => x.ErrorCode)
            .HasMaxLength(100);

        // Relationship: Payment → Client (many-to-one)
        builder.HasOne(x => x.Client)
            .WithMany(c => c.Payments) // ensure Client entity has ICollection<Payment> Payments
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Payment → PaymentMethod (many-to-one)
        builder.HasOne(x => x.PaymentMethod)
            .WithMany(c => c.Payments) // ensure PaymentMethod entity has ICollection<Payment> Payments
            .HasForeignKey(x => x.PaymentMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Payment → PaymentRefund (one-to-many)
        builder.HasMany(x => x.Refunds)
            .WithOne(r => r.Payment)
            .HasForeignKey(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional: unique index on provider payment ID
        builder.HasIndex(x => x.ProviderPaymentId).IsUnique();
    }
}
