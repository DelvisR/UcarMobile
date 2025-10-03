using System.Linq;
using Microsoft.EntityFrameworkCore;
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
            var clrType = entityType.ClrType;
            var builder = modelBuilder.Entity(clrType);

            // Only define HasKey in root types (without base type)
            // Avoid adding the key to User's subordinates so that you can create the 1:1 relationship with client, Technician, etc.
            if (entityType.BaseType == null) // is root
            {
                builder.HasKey("Id");
            }

            builder.Property("CreatedBy").HasMaxLength(40).IsRequired();
            builder.Property("CreatedDate").IsRequired();
            builder.Property("LastModifiedBy").HasMaxLength(40).IsRequired();
            builder.Property("LastModifiedDate").IsRequired();
            builder.Property("IsDeleted").IsRequired().HasDefaultValue(false);
        }
    }
}
