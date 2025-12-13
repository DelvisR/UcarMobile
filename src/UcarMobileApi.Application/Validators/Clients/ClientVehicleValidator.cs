using FluentValidation;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Validators.Appointments;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Clients;

public class ClientVehicleCreateDtoValidator : AbstractValidator<ClientVehicleCreateDto>
{
    public ClientVehicleCreateDtoValidator()
    {
        RuleFor(x => x.VehicleId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        // ClientId is optional but if provided must be valid
        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero)
            .When(x => x.ClientId.HasValue);

        // No required fields because ClientVehicle may be newly created
        // but at least one identifier must logically exist (VehicleId)

        RuleForEach(x => x.Services)
            .SetValidator(new AppointmentServiceCreateDtoValidator());

        RuleForEach(x => x.Parts)
            .SetValidator(new AppointmentPartCreateDtoValidator());
    }
}
