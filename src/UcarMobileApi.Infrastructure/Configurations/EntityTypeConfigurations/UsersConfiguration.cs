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

        builder.HasIndex(u => u.CognitoId).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.CognitoId).HasMaxLength(256).IsRequired();
        builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);
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

// Permission entity configuration
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasIndex(p => p.Name).IsUnique();

        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(256);
    }
}

// UserRole entity configuration (many-to-many between User and Role)
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

// RolePermission entity configuration (many-to-many between Role and Permission)
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rp => rp.Permission).WithMany(p => p.RolePermissions).HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}