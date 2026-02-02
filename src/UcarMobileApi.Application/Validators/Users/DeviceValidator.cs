using FluentValidation;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Users;

public class DeviceValidator : AbstractValidator<RegisterDeviceDto>
{
    public DeviceValidator()
    {
        RuleFor(d => d.Token).NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(500).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 500));

        RuleFor(d => d.Platform).IsInEnum().WithMessage(ValidatorErrors.InvalidValue);
    }
}
