using FluentValidation;
using UcarMobileApi.Application.DTOs.Common;

namespace UcarMobileApi.Application.Validators.Common;

public class AddressInfoValidator : AbstractValidator<AddressInfoDto>
{
    public AddressInfoValidator()
    {
        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(255).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 255));

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(10).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 10));

        RuleFor(x => x.Lat)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Lng)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees.");
    }
}
