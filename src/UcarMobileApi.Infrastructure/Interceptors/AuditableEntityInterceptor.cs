using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UcarMobileApi.Core.Entities;

namespace UcarMobileApi.Infrastructure.Interceptors;

public class AuditInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditInfo(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<EntityBase>();

        var modifiedOrCreatedBy = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entry.State is EntityState.Added)
            {
                entity.CreatedDate = DateTime.UtcNow;
                entity.CreatedBy = modifiedOrCreatedBy;
            }
            else //Avoid modifications of these fields in the updates. They may come blank as a product of AutoMappers
            {
                context.Entry(entity).Property(p => p.CreatedDate).IsModified = false;
                context.Entry(entity).Property(p => p.CreatedBy).IsModified = false;
            }
            entity.LastModifiedDate = DateTime.UtcNow;
            entity.LastModifiedBy = modifiedOrCreatedBy;
        }
    }
}
