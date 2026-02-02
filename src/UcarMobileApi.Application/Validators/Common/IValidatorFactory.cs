using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace UcarMobileApi.Application.Validators.Common;

public interface IValidatorResolver
{
    IValidator<T> Get<T>();
}

public sealed class FluentValidatorFactory(IServiceProvider sp) : IValidatorResolver
{
    public IValidator<T> Get<T>()
        => sp.GetRequiredService<IValidator<T>>();
}
