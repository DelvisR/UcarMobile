using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Appointments;

public abstract class AppointmentServiceUpsertDtoValidator<T> : AbstractValidator<T> where T : AppointmentServiceUpsertDto
{
    protected AppointmentServiceUpsertDtoValidator()
    {
        RuleFor(x => x.ServiceCategoryId)
        .GreaterThan(0)
        .WithMessage(ValidatorErrors.GreaterThanZero)
        .When(x => x.ServiceCategoryId.HasValue);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage(ValidatorErrors.GreaterThanZero);

        // ServiceId or CustomService
        RuleFor(x => x)
            .Must(x => x.ServiceId.HasValue || !string.IsNullOrWhiteSpace(x.CustomService))
            .WithMessage("You must specify either ServiceId or CustomService.");

        // EXACTLY ONE of ServiceId or CustomService
        RuleFor(x => x)
            .Must(x => !(x.ServiceId.HasValue && !string.IsNullOrWhiteSpace(x.CustomService)))
            .WithMessage("You cannot specify both ServiceId and CustomService.");


        // If ServiceId is present → must be > 0
        RuleFor(x => x.ServiceId!.Value)
            .GreaterThan(0)
            .WithMessage(ValidatorErrors.GreaterThanZero)
            .When(x => x.ServiceId.HasValue);

        // If CustomService is present → length constraint
        RuleFor(x => x.CustomService!)
            .MaximumLength(200)
            .WithMessage(ValidatorErrors.MaxLengthExceeded)
            .When(x => !string.IsNullOrWhiteSpace(x.CustomService));
    }
}

public sealed class AppointmentServiceCreateDtoValidator : AppointmentServiceUpsertDtoValidator<AppointmentServiceCreateDto>
{
    public AppointmentServiceCreateDtoValidator()
    {
        RuleForEach(x => x.Parts)
            .SetValidator(new AppointmentPartCreateDtoValidator());
    }
}

public sealed class AppointmentServiceUpdateDtoValidator : AppointmentServiceUpsertDtoValidator<AppointmentServiceUpdateDto>;
