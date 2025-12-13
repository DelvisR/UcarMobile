using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Common;

public static class OwnedTypeExtensions
{
    public static OwnedNavigationBuilder<TEntity, AddressInfo> ConfigureAddressInfo<TEntity>(this OwnedNavigationBuilder<TEntity, AddressInfo> builder)
        where TEntity : class
    {
        builder.Property(a => a.FullAddress).HasColumnName("FullAddress")
            .HasMaxLength(255).IsRequired().HasDefaultValue("3400 14th Street Plano, TX"); ;

        builder.Property(a => a.ZipCode).HasColumnName("ZipCode")
            .HasMaxLength(10).IsRequired().HasDefaultValue("75074"); ;

        builder.Property(a => a.BasePoint)
            .HasColumnType("geometry(Point, 4326)")
            .HasDefaultValueSql("ST_SetSRID(ST_MakePoint(-96.6702438, 33.0146527), 4326)");

        builder.HasIndex(a => a.BasePoint)
            .HasMethod("GIST");

        builder.WithOwner();

        return builder;
    }
}
