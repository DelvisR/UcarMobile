using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Services.Users;

/// <summary>
/// Action service
/// </summary>
public class ActionService(IMapper mapper, IAppDbContext context)
{
    /// <summary>
    /// Retrieves all actions.
    /// </summary>
    public async Task<(IHeaderDictionary, IEnumerable<ActionDto>)> GetAllAsync(QueryFilter query, CancellationToken ct)
    {
        var actions = context.Set<Action>().AsNoTracking();

        // AutoMapper ProjectTo + Filtering + Ordering + Paging
        var qp = await actions.GridifyQueryableAsync(query, null, ct);
        return (qp.GeneratePaginationHttpHeaders(), await qp.Query.ProjectTo<ActionDto>(mapper.ConfigurationProvider).ToListAsync(ct));
    }
}
