using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Infrastructure.Interceptors;

public class PersistenceInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        TrackDeletedFilesFromEntities(eventData.Context).GetAwaiter().GetResult();
        ApplyAuditInfo(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await TrackDeletedFilesFromEntities(eventData.Context, cancellationToken);
        ApplyAuditInfo(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditInfo(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<AuditableEntity>().Where(e => e.State is EntityState.Added or EntityState.Modified);

        if (!entries.Any()) return;

        var user = httpContextAccessor?.HttpContext?.User;

        var modifiedOrCreatedBy = user?.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ??
                                  user?.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                                  user?.Identity?.Name ?? "System";

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

    /// <summary>
    /// We search for all entities that have a relationship with StoredFile to mark it as Logical Delete for subsequent cleanup in the cloud.
    /// </summary>
    private static async Task TrackDeletedFilesFromEntities(DbContext? context, CancellationToken ct = default)
    {
        if (context == null) return;

        var storedFilesToDelete = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted && e.Metadata.FindNavigation("StoredFile") != null)
            .ToList();

        if (storedFilesToDelete.Count == 0) return;

        foreach (var entry in storedFilesToDelete)
        {
            var reference = entry.Reference("StoredFile");

            if (reference.CurrentValue == null)
            {
                entry.State = EntityState.Unchanged; // We changed the status to allow loading.
                try
                {
                    if (!reference.IsLoaded) // We only load it if it is not in memory (perhaps Include(StoredFile) was used).
                    {
                        await reference.LoadAsync(ct);
                    }
                }
                finally
                {
                    entry.State = EntityState.Deleted; // We restored it to its original condition.
                }
            }

            if (reference.CurrentValue is not StoredFile storedFile) continue;

            storedFile.IsDeleted = true;
            storedFile.DeleteStatus = DeleteStatus.Pending;
            context.Entry(storedFile).State = EntityState.Modified; // make sure EF knows that it has changed
        }
    }
}
