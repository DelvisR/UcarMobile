using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Clients;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Client"); // TPT (Table-Per-Type)

        builder.Property(c => c.Address).HasMaxLength(256);
    }
}
