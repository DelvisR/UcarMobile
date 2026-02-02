using System;
using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Appointments;

// Validator for appointment creation
public class AppointmentCreateDtoValidator : AbstractValidator<AppointmentCreateDto>
{
    public AppointmentCreateDtoValidator()
    {
        RuleFor(a => a.ClientId)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(a => a.ScheduledStart)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("ScheduledStart must be in the present or future.");

        RuleFor(a => a.ServiceAddress)
            .SetValidator(new AddressInfoValidator());

        RuleFor(a => a.EstimatedTotal)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(x => x.PaymentMethodId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero)
            .When(x => x.PaymentMethodId.HasValue);

        RuleFor(x => x.Vehicles)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired);

        RuleForEach(x => x.Vehicles)
            .SetValidator(new AppointmentVehicleCreateDtoValidator());

        RuleForEach(x => x.Notes)
            .SetValidator(new AppointmentNoteCreateDtoValidator());
    }
}

public class UpdateAppointmentRequestValidator : AbstractValidator<UpdateAppointmentRequest>
{
    public UpdateAppointmentRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ValidatorErrors.InvalidValue)
            .When(x => x.Status.HasValue);

        RuleFor(x => x.EstimatedTotal)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero)
            .When(x => x.EstimatedTotal.HasValue);

        RuleFor(a => a.ScheduledStart)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("ScheduledStart must be in the present or future.")
            .When(x => x.ScheduledStart.HasValue);

        RuleFor(x => x.ScheduledEnd)
            .GreaterThan(x => x.ScheduledStart).WithMessage("ScheduledEnd must be greater than ScheduledStart")
            .When(x => x.ScheduledStart.HasValue && x.ScheduledEnd.HasValue);

        RuleFor(x => x.ServiceAddress)
            .Custom((address, context) =>
            {
                var validator = new AddressInfoValidator();
                var result = validator.Validate(address!);

                if (result.IsValid) return;

                foreach (var error in result.Errors)
                {
                    context.AddFailure($"ServiceAddress.{error.PropertyName}", error.ErrorMessage);
                }
            })
            .When(x => x.ServiceAddress != null);

        RuleFor(x => x.PaymentMethodId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero)
            .When(x => x.PaymentMethodId.HasValue);

        RuleFor(c => c.CancellationReason)
            .MaximumLength(250).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 250));
    }
}

public class RescheduleRequestValidator : AbstractValidator<RescheduleRequest>
{
    public RescheduleRequestValidator()
    {


        RuleFor(a => a.ScheduledStart)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("ScheduledStart must be in the present or future.");

        RuleFor(x => x.ScheduledEnd)
            .GreaterThan(x => x.ScheduledStart).WithMessage("ScheduledEnd must be greater than ScheduledStart")
            .When(x => x.ScheduledEnd.HasValue);
    }
}

public sealed class CompleteAppointmentRequestValidator : AbstractValidator<CompleteAppointmentRequest>
{
    public CompleteAppointmentRequestValidator()
    {
        RuleFor(x => x.Vehicles)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired);

        RuleForEach(x => x.Vehicles)
            .ChildRules(v =>
            {
                v.RuleFor(x => x.Id)
                    .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

                v.RuleFor(x => x.OdometerKm)
                    .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative);
            });

        RuleFor(x => x.WarrantyMonths)
            .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative)
            .When(x => x.WarrantyMonths.HasValue);

        RuleFor(x => x.WarrantyMiles)
            .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative)
            .When(x => x.WarrantyMiles.HasValue);
    }
}


