using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Payments;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Payments;

/// <summary>
/// Entity Framework configuration for the <see cref="PaymentMethod"/> entity.
/// Defines schema, constraints, and relationships.
/// </summary>
public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        // Optional table name
        // builder.ToTable("PaymentMethod");

        builder.Property(x => x.ProviderPaymentMethodId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Brand)
            .HasMaxLength(32)
            .IsRequired(false);

        builder.Property(x => x.Last4)
            .HasMaxLength(4)
            .IsRequired(false);

        builder.Property(x => x.ExpMonth)
            .IsRequired();

        builder.Property(x => x.ExpYear)
            .IsRequired();

        builder.Property(x => x.IsDefault)
            .HasDefaultValue(false);

        // Relationship: PaymentMethod → Client
        builder.HasOne(x => x.Client)
            .WithMany(c => c.PaymentMethods)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional: unique index per client on provider method
        builder.HasIndex(x => new { x.ClientId, x.ProviderPaymentMethodId })
            .IsUnique();
    }
}
