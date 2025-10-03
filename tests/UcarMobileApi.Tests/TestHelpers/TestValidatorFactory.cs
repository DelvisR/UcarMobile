using System.Threading;
using FluentValidation;
using Moq;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Tests.TestHelpers;

public static class TestValidatorFactory
{
    public static IValidator<UserDto> CreateAlwaysValidUserValidator()
    {
        var v = new Mock<IValidator<UserDto>>();
        v.Setup(x => x.ValidateAsync(It.IsAny<UserDto>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(new FluentValidation.Results.ValidationResult()); // always valid
        return v.Object;
    }
}
