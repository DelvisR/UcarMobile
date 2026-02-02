using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasOne(d => d.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.Token, x.Platform }).IsUnique();

        builder.Property(d => d.UserId).IsRequired();
        builder.Property(d => d.Token).IsRequired().HasMaxLength(500);
        builder.Property(d => d.EndpointArn).IsRequired().HasMaxLength(500);
        builder.Property(d => d.Platform).IsRequired();
    }
}
