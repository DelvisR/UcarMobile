using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Services.Configurations;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Application.Validators.Clients;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Services.Clients;

public class ClientService(IMapper mapper, IAppDbContext context, BusinessParameterService businessParameters, IGridifyMapper<Client> gridifymapper)
{
    public async Task<(IHeaderDictionary, IEnumerable<ClientDto>)> GetClientsAsync(QueryFilter query, CancellationToken ct)
    {
        var clients = context.Set<Client>().AsNoTracking();

        // AutoMapper ProjectTo + Filtering + Ordering + Paging
        var qp = await clients.GridifyQueryableAsync(query, gridifymapper, ct);
        return (qp.GeneratePaginationHttpHeaders(), await qp.Query.ProjectTo<ClientDto>(mapper.ConfigurationProvider).ToListAsync(ct));
    }

    public async Task<ClientDto?> GetClientAsync(int id, CancellationToken ct)
    {
        return await context.Set<Client>()
            .AsNoTracking()
            .Where(u => u.Id == id)
            .ProjectTo<ClientDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ClientDto?> GetClientByEmailAsync(string email, CancellationToken ct)
    {
        return await context.Set<Client>()
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.Email, email))
            .ProjectTo<ClientDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }


    public async Task CreateClientAsync(ClientDto dto, CancellationToken ct)
    {
        var validator = new ClientValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var client = mapper.Map<Client>(dto);

        var defaultRoles = await businessParameters.GetValueAsync<string>("DefaultClientRoles", ct);

        if (!string.IsNullOrWhiteSpace(defaultRoles))
        {
            var roleIds = defaultRoles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse);

            client.UserRoles = [.. roleIds.Select(rid => new UserRole { RoleId = rid })];
        }

        context.Set<Client>().Add(client);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateClientAsync(int id, ClientDto dto, CancellationToken ct)
    {
        var validator = new ClientValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var client = await context.Set<Client>().FirstOrDefaultAsync(c => c.Id == id, ct);

        if (client != null)
        {
            mapper.Map(dto, client);
            await context.SaveChangesAsync(ct);
        }
        else
        {
            throw new KeyNotFoundException($"Client with ID {id} not found.");
        }
    }
}
