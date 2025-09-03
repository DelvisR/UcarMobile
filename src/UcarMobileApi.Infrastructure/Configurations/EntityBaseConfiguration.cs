using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities;

namespace UcarMobileApi.Infrastructure.Configurations
{
    public class EntityBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : EntityBase
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn()
                .HasColumnOrder(0);

            // Order starts at 100 to leave room for entity-specific props first
            builder.Property(e => e.CreatedBy)
                .HasMaxLength(40)
                .IsRequired()
                .HasColumnOrder(100);

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasColumnOrder(101);

            builder.Property(e => e.LastModifiedBy)
                .HasMaxLength(40)
                .IsRequired()
                .HasColumnOrder(102);

            builder.Property(e => e.LastModifiedDate)
                .IsRequired()
                .HasColumnOrder(103);

            builder.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnOrder(104);
        }
    }
}