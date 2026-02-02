using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Appointments;

public abstract class AppointmentPartUpsertDtoValidator<T> : AbstractValidator<T> where T : AppointmentPartUpsertDto
{
    protected AppointmentPartUpsertDtoValidator()
    {
        RuleFor(x => x.PartName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(150)
            .WithMessage(ValidatorErrors.MaxLengthExceeded);

        RuleFor(x => x.PartNumber!)
            .MaximumLength(50)
            .WithMessage(ValidatorErrors.MaxLengthExceeded)
            .When(x => !string.IsNullOrWhiteSpace(x.PartNumber));

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative);

        RuleFor(x => x.Note!)
            .MaximumLength(500)
            .WithMessage(ValidatorErrors.MaxLengthExceeded)
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}

public sealed class AppointmentPartCreateDtoValidator : AppointmentPartUpsertDtoValidator<AppointmentPartCreateDto>;
public sealed class AppointmentPartUpdateDtoValidator : AppointmentPartUpsertDtoValidator<AppointmentPartUpdateDto>;
