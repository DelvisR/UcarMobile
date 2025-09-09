using Microsoft.EntityFrameworkCore;
using System.Linq;
using UcarMobileApi.Core.Entities;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

public static class ModelBuilderExtensions
{
    public static void ApplyEntityBaseConfiguration(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(EntityBase).IsAssignableFrom(t.ClrType));

        foreach (var entityType in entityTypes)
        {
            // Only apply common properties if they have not already been configured
            var builder = modelBuilder.Entity(entityType.ClrType);

            // EntityBase base configuration
            builder.HasKey("Id");

            builder.Property("CreatedBy").HasMaxLength(40).IsRequired();
            builder.Property("CreatedDate").IsRequired();
            builder.Property("LastModifiedBy").HasMaxLength(40).IsRequired();
            builder.Property("LastModifiedDate").IsRequired();
            builder.Property("IsDeleted").IsRequired().HasDefaultValue(false);
        }
    }
}