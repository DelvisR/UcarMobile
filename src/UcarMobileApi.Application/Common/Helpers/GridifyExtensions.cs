using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gridify;
using Gridify.EntityFramework;
using UcarMobileApi.Application.Common.Models;

namespace UcarMobileApi.Application.Common.Helpers;

public static class GridifyExtensions
{
    public static Task<QueryablePaging<T>> GridifySafeAsync<T>(this IQueryable<T> query, QueryFilter filter, IGridifyMapper<T>? mapper = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filter.OrderBy))
        {
            // ALL your entities have an ID (because they inherit from EntityBase).
            filter.OrderBy = "Id";
        }

        return query.GridifyQueryableAsync(filter, mapper, ct);
    }
}
