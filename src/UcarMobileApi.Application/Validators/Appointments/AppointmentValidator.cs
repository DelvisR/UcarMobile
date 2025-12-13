using System;
using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Clients;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Appointments;

// Validator for appointment creation
public class AppointmentCreateDtoValidator : AbstractValidator<AppointmentCreateDto>
{
    public AppointmentCreateDtoValidator()
    {
        RuleFor(a => a.ClientId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(a => a.ScheduledStart)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("ScheduledStart must be in the present or future.");

        RuleFor(a => a.ServiceAddress)
            .SetValidator(new AddressInfoValidator());

        RuleFor(x => x.Vehicles)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired);

        RuleForEach(x => x.Vehicles)
            .SetValidator(new ClientVehicleCreateDtoValidator());
    }
}

public class AppointmentServiceCreateDtoValidator : AbstractValidator<AppointmentServiceCreateDto>
{
    public AppointmentServiceCreateDtoValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative);
    }
}

public class AppointmentPartCreateDtoValidator : AbstractValidator<AppointmentPartCreateDto>
{
    public AppointmentPartCreateDtoValidator()
    {
        RuleFor(x => x.PartName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative);
    }
}



