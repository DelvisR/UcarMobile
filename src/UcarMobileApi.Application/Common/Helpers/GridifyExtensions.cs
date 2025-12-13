using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gridify;
using Gridify.EntityFramework;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Application.Common.Helpers;

public static class GridifyExtensions
{
    public static Task<QueryablePaging<T>> GridifySafeAsync<T>(this IQueryable<T> query, QueryFilter filter, IGridifyMapper<T>? mapper = null,
        CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(filter.OrderBy)) return query.GridifyQueryableAsync(filter, mapper, ct);

        if (typeof(EntityBase).IsAssignableFrom(typeof(T)))
        {
            filter.OrderBy = "Id";
        }
        else
        {
            var firstProperty = typeof(T).GetProperties().FirstOrDefault(p => p.CanRead);

            filter.OrderBy = firstProperty?.Name ?? "Id";
        }

        return query.GridifyQueryableAsync(filter, mapper, ct);
    }
}
