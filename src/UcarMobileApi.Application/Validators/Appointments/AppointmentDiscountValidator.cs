using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Common;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.Validators.Appointments;

public class ApplyDiscountDtoValidator : AbstractValidator<ApplyDiscountDto>
{
    public ApplyDiscountDtoValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum().WithMessage(ValidatorErrors.InvalidValue);

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage(ValidatorErrors.InvalidValue);

        RuleFor(x => x.Source)
            .IsInEnum().WithMessage(ValidatorErrors.InvalidValue);

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(200).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 200));

        When(x => x.Category == DiscountCategory.Coupon, () =>
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
                .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));
        });
    }
}

