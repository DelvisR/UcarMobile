using FluentValidation;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators;

public class ServiceZoneValidator : AbstractValidator<ServiceZoneDto>
{
    public ServiceZoneValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(150).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 150));
        RuleFor(x => x.BaseAddress)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(150).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 250));
        RuleFor(x => x.RadiusMiles).GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
        RuleForEach(x => x.ZipCodes).Matches(@"^\d{5}$").WithMessage("Invalid ZIP code format");
    }
}
