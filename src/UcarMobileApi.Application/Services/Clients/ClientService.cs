using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Gridify;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Helpers;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Application.Validators.Common;
using UcarMobileApi.Core.Constants;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Services.Clients;

public class ClientService(IMapper mapper, IAppDbContext context, BusinessParameterService businessParameters,
    IGridifyMapper<Client> gridifymapper, IValidatorResolver validatorResolver)
{
    public async Task<(IHeaderDictionary, IEnumerable<ClientDto>)> GetClientsAsync(QueryFilter query, CancellationToken ct)
    {
        var clients = context.Set<Client>().AsNoTracking();

        // AutoMapper ProjectTo + Filtering + Ordering + Paging
        var qp = await clients.GridifySafeAsync(query, gridifymapper, ct);
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
        await validatorResolver.Get<ClientDto>().ValidateAndThrowAsync(dto, ct);

        var client = mapper.Map<Client>(dto);

        var defaultRoles = await businessParameters.GetValueAsync<string>(BusinessParameterKeys.DefaultClientRoles, ct);

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
        await validatorResolver.Get<ClientDto>().ValidateAndThrowAsync(dto, ct);

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

    #region ClientVehicle methods

    public async Task<IEnumerable<ClientVehicleDto>> GetClientVehiclesAsync(int clientId, CancellationToken ct)
    {
        return await context.Set<ClientVehicle>()
            .AsNoTracking()
            .Where(cv => cv.ClientId == clientId)
            .OrderByDescending(cv => cv.Id)
            .ProjectTo<ClientVehicleDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ClientVehicleDto>> GetVehiclesAsync(string authProviderId, CancellationToken ct)
    {
        return await context.Set<ClientVehicle>()
            .AsNoTracking()
            .Where(cv => cv.Client.AuthProviderId == authProviderId)
            .OrderByDescending(cv => cv.Id)
            .ProjectTo<ClientVehicleDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<ClientVehicleDto?> GetClientVehicleAsync(int clientId, int id, CancellationToken ct)
    {
        return await context.Set<ClientVehicle>()
            .AsNoTracking()
            .Where(cv => cv.ClientId == clientId && cv.Id == id)
            .ProjectTo<ClientVehicleDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ClientVehicleDto?> GetVehicleAsync(string authProviderId, int id, CancellationToken ct)
    {
        return await context.Set<ClientVehicle>()
            .AsNoTracking()
            .Where(cv => cv.Client.AuthProviderId == authProviderId && cv.Id == id)
            .ProjectTo<ClientVehicleDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CreateClientVehicleAsync(int clientId, ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<ClientVehicleUpsertDto>().ValidateAndThrowAsync(dto, ct);

        var clientExists = await context.Set<Client>()
            .AsNoTracking()
            .AnyAsync(c => c.Id == clientId, ct);

        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {clientId} not found.");

        await CreateVehicleInternalAsync(clientId, dto, ct);
    }

    public async Task CreateVehicleAsync(string authProviderId, ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<ClientVehicleUpsertDto>().ValidateAndThrowAsync(dto, ct);

        var client = await context.Set<Client>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, ct);

        if (client == null)
            throw new KeyNotFoundException($"Client with AuthProviderId not found.");

        await CreateVehicleInternalAsync(client.Id, dto, ct);
    }

    private async Task CreateVehicleInternalAsync(int clientId, ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        var entity = mapper.Map<ClientVehicle>(dto);
        entity.ClientId = clientId;

        context.Set<ClientVehicle>().Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateClientVehicleAsync(int clientId, int id, ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<ClientVehicleUpsertDto>().ValidateAndThrowAsync(dto, ct);

        var entity = await context.Set<ClientVehicle>().FirstOrDefaultAsync(cv => cv.Id == id && cv.ClientId == clientId, ct)
            ?? throw new KeyNotFoundException($"ClientVehicle with ID {id} for client {clientId} not found.");

        // Map editable fields from DTO to entity
        mapper.Map(dto, entity);
        // Ensure ClientId isn't changed
        entity.ClientId = clientId;

        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateVehicleAsync(string authProviderId, int vehicleId, ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<ClientVehicleUpsertDto>().ValidateAndThrowAsync(dto, ct);

        var entity = await context.Set<ClientVehicle>()
                         .FirstOrDefaultAsync(cv => cv.Id == vehicleId && cv.Client.AuthProviderId == authProviderId, ct)
                     ?? throw new KeyNotFoundException($"ClientVehicle with ID {vehicleId} not found for current user.");

        mapper.Map(dto, entity);
        // ClientId is already correct, no need to reassign

        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateClientVehiclePatchAsync(int clientId, int id, ClientVehicleUpdateDto dto, CancellationToken ct)
    {
        // validate if a validator exists for the patch DTO
        await validatorResolver.Get<ClientVehicleUpdateDto>().ValidateAndThrowAsync(dto, ct);

        var entity = await context.Set<ClientVehicle>()
                         .FirstOrDefaultAsync(cv => cv.Id == id && cv.ClientId == clientId, ct)
                     ?? throw new KeyNotFoundException($"ClientVehicle with ID {id} for client {clientId} not found.");

        // Mapping uses IgnoreNullValuesForPatch so only provided properties are applied
        mapper.Map(dto, entity);

        // Ensure ClientId cannot be changed via patch
        entity.ClientId = clientId;

        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateVehiclePatchAsync(string authProviderId, int vehicleId, ClientVehicleUpdateDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<ClientVehicleUpdateDto>().ValidateAndThrowAsync(dto, ct);

        var entity = await context.Set<ClientVehicle>()
                         .FirstOrDefaultAsync(cv => cv.Id == vehicleId && cv.Client.AuthProviderId == authProviderId, ct)
                     ?? throw new KeyNotFoundException($"ClientVehicle with ID {vehicleId} not found for current user.");

        mapper.Map(dto, entity);

        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteClientVehicleAsync(int clientId, int id, CancellationToken ct)
    {
        var entity = await context.Set<ClientVehicle>().FirstOrDefaultAsync(cv => cv.Id == id && cv.ClientId == clientId, ct)
            ?? throw new KeyNotFoundException($"ClientVehicle with ID {id} for client {clientId} not found.");

        context.Set<ClientVehicle>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteVehicleAsync(string authProviderId, int vehicleId, CancellationToken ct)
    {
        var entity = await context.Set<ClientVehicle>()
                         .FirstOrDefaultAsync(cv => cv.Id == vehicleId && cv.Client.AuthProviderId == authProviderId, ct)
                     ?? throw new KeyNotFoundException($"ClientVehicle with ID {vehicleId} not found for current user.");

        context.Set<ClientVehicle>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion
}
