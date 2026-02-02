using System;
using Gridify;
using Microsoft.Extensions.DependencyInjection;
using UcarMobileApi.Application.Common.Interfaces;

namespace UcarMobileApi.Infrastructure.Configurations.Gridify;

public sealed class GridifyMapperResolver(IServiceProvider sp) : IGridifyMapperResolver
{
    public IGridifyMapper<T> Get<T>()
    {
        var mapper = sp.GetService<IGridifyMapper<T>>();
        return mapper is null ? throw new InvalidOperationException($"No GridifyMapper registered for {typeof(T).Name}") : mapper;
    }
}


