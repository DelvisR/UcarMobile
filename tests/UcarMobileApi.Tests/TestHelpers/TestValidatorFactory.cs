using FluentValidation;
using Moq;
using System.Threading;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Tests.TestHelpers;

public static class TestValidatorFactory
{
    public static IValidator<CreateUserDto> CreateAlwaysValidUserValidator()
    {
        var v = new Mock<IValidator<CreateUserDto>>();
        v.Setup(x => x.ValidateAsync(It.IsAny<CreateUserDto>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(new FluentValidation.Results.ValidationResult()); // always valid
        return v.Object;
    }
}