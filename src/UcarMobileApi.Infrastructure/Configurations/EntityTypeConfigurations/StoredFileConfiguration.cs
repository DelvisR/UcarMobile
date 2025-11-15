using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

/// <summary>
/// EF Core configuration for StoredFile entity.
/// </summary>
public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.Property(x => x.Bucket)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Key)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();

        builder.HasIndex(x => x.FileName);
    }
}
