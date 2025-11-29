using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

// User entity configuration
// By default, EF creates tables with the same name as the entity without pluralizing it.
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("UserAccount"); // Rename because “User” is a reserved word in PostgreSQL

        // Primary key (in User)
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.AuthProviderId).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(10).IsRequired();
        builder.Property(u => u.AuthProviderId).HasMaxLength(256);
        builder.Property(x => x.LangKey).HasMaxLength(6).HasDefaultValue("en");
        builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(false);
    }
}

// Role entity configuration
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(256);
    }
}

// Action entity configuration
public class ActionConfiguration : IEntityTypeConfiguration<Action>
{
    public void Configure(EntityTypeBuilder<Action> builder)
    {
        builder.HasIndex(p => p.Name).IsUnique();

        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(256);
        builder.Property(p => p.Resource).HasMaxLength(50).IsRequired().HasDefaultValue("RESOURCE_DEFAULT");
    }
}

// UserRole entity configuration (many-to-many between User and Role)
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

// RoleAction entity configuration (many-to-many between Role and Action)
public class RoleActionConfiguration : IEntityTypeConfiguration<RoleAction>
{
    public void Configure(EntityTypeBuilder<RoleAction> builder)
    {
        builder.HasOne(rp => rp.Role).WithMany(r => r.RoleActions).HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rp => rp.Action).WithMany(p => p.RoleActions).HasForeignKey(rp => rp.ActionId).OnDelete(DeleteBehavior.Cascade);
    }
}
