using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Helpers;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.ServiceZone;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Application.Validators.ServiceZone;

namespace UcarMobileApi.Application.Services.ServiceZone;

/// <summary>
/// Service responsible for managing service zones and validating whether a location is covered.
/// </summary>
public class ServiceZoneService(IAppDbContext context, IMapper mapper, ILocationService location, ICacheService cache)
{
    private const string CacheKey = "ServiceZones";

    /// <summary>
    /// Creates a new service zone from the specified DTO, performing geocoding for the base address.
    /// </summary>
    public async Task<ServiceZoneDto> CreateAsync(ServiceZoneDto dto, CancellationToken ct)
    {
        var validator = new ServiceZoneValidator();
        await validator.ValidateAndThrowAsync(dto, ct);

        var geo = await location.GetCoordinatesFromAddressAsync(dto.BaseAddress, ct)
                  ?? throw new InvalidOperationException("Invalid BaseAddress. Cannot geocode.");

        if (!string.IsNullOrWhiteSpace(geo.Zip) && !dto.ZipCodes.Contains(geo.Zip))
            dto.ZipCodes.Add(geo.Zip);

        var zone = mapper.Map<Core.Entities.ServiceZone.ServiceZone>(dto);
        zone.Lat = geo.Lat;
        zone.Lng = geo.Lng;

        context.Set<Core.Entities.ServiceZone.ServiceZone>().Add(zone);
        await context.SaveChangesAsync(ct);
        await cache.InvalidateAsync(CacheKey);

        return mapper.Map<ServiceZoneDto>(zone);
    }

    /// <summary>
    /// Updates an existing service zone, re-geocoding its base address if modified.
    /// </summary>
    public async Task<ServiceZoneDto?> UpdateAsync(int id, ServiceZoneDto dto, CancellationToken ct)
    {
        var validator = new ServiceZoneValidator();
        await validator.ValidateAndThrowAsync(dto, ct);

        var zone = await context.Set<Core.Entities.ServiceZone.ServiceZone>().FirstOrDefaultAsync(z => z.Id == id, ct)
            ?? throw new KeyNotFoundException($"ServiceZone with ID {id} not found.");

        var geo = await location.GetCoordinatesFromAddressAsync(dto.BaseAddress, ct)
            ?? throw new InvalidOperationException("Invalid BaseAddress. Cannot geocode.");

        if (!string.IsNullOrWhiteSpace(geo.Zip) && !dto.ZipCodes.Contains(geo.Zip))
            dto.ZipCodes.Add(geo.Zip);

        mapper.Map(dto, zone);
        zone.Lat = geo.Lat;
        zone.Lng = geo.Lng;

        await context.SaveChangesAsync(ct);
        await cache.InvalidateAsync(CacheKey);

        return mapper.Map<ServiceZoneDto>(zone);
    }

    /// <summary>
    /// Retrieves all service zones (cached for performance).
    /// </summary>
    public async Task<List<ServiceZoneDto>> GetAllAsync(CancellationToken ct = default)
    {
        var zones = await cache.GetOrSetAsync(
            CacheKey,
            async () => await context.Set<Core.Entities.ServiceZone.ServiceZone>().AsNoTracking().ToListAsync(ct),
            TimeSpan.FromHours(24), null);

        return mapper.Map<List<ServiceZoneDto>>(zones);
    }

    /// <summary>
    /// Retrieves a service zone by its identifier.
    /// </summary>
    public async Task<ServiceZoneDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var zone = await context.Set<Core.Entities.ServiceZone.ServiceZone>().FindAsync([id], ct);
        return zone == null ? null : mapper.Map<ServiceZoneDto>(zone);
    }

    // ----------------------------------------------------------
    // VALIDATION
    // ----------------------------------------------------------

    /// <summary>
    /// Checks if an address string is within a service zone.
    /// Returns result with geocoded coordinates and ZIP.
    /// </summary>
    public async Task<AddressValidationResult> ValidateAddressAsync(string address, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required.", nameof(address));

        var geo = await location.GetCoordinatesFromAddressAsync(address, ct);
        if (geo == null)
            return new AddressValidationResult(false, 0, 0, string.Empty);

        return await ValidateLatLngZipAsync(geo.Lat, geo.Lng, geo.Zip, ct);
    }

    /// <summary>
    /// Checks if a place/address ID is within a service zone.
    /// Returns result with coordinates and ZIP.
    /// </summary>
    public async Task<AddressValidationResult> ValidateAddressByIdAsync(string addressId, CancellationToken ct = default)
    {
        var geo = await location.GetPlaceLocationAsync(addressId, ct);
        if (geo == null)
            return new AddressValidationResult(false, 0, 0, string.Empty);

        return await ValidateLatLngZipAsync(geo.Lat, geo.Lng, geo.Zip, ct);
    }

    /// <summary>
    /// Validates coordinates and postal code directly.
    /// Returns result with lat/lng/zip and whether it's inside a zone.
    /// </summary>
    public async Task<AddressValidationResult> ValidateAddressAsync(double lat, double lng, string zip, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(zip))
            throw new ArgumentException("ZIP code is required.", nameof(zip));

        return await ValidateLatLngZipAsync(lat, lng, zip, ct);
    }

    /// <summary>
    /// Core routine: loads service zones, filters by ZIP, and checks distance.
    /// </summary>
    private async Task<AddressValidationResult> ValidateLatLngZipAsync(double lat, double lng, string zip, CancellationToken ct)
    {
        var zones = await GetAllAsync(ct);
        if (zones.Count == 0)
            return new AddressValidationResult(false, lat, lng, zip);

        // Filter by active zones and matching ZIP
        var zone = zones
            .Where(z => z is { IsActive: true } && z.ZipCodes.Contains(zip))
            .FirstOrDefault(z =>
            {
                var distance = GeoHelper.DistanceMiles(z.Lat, z.Lng, lat, lng);
                return distance <= z.RadiusMiles;
            });

        return new AddressValidationResult(zone != null, lat, lng, zip);
    }
}
