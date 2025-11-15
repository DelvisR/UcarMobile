using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

/// <summary>
/// EF Core fluent configuration for the Lead entity.
/// Keeps DB schema details centralized.
/// </summary>
public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        // Table name
        builder.ToTable("Lead");

        // Columns
        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // Create an index on Phone for faster lookups
        builder.HasIndex(x => x.Phone).HasDatabaseName("IX_Lead_Phone");
    }
}
