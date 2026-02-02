using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Clients;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Appointments;

public class AppointmentVehicleCreateDtoValidator : AbstractValidator<AppointmentVehicleCreateDto>
{
    public AppointmentVehicleCreateDtoValidator()
    {
        RuleFor(x => x.Vehicle)
            .NotNull().WithMessage(ValidatorErrors.IsRequired)
            .SetValidator(new ClientVehicleUpsertDtoValidator());

        RuleForEach(x => x.Services)
            .SetValidator(new AppointmentServiceCreateDtoValidator());
    }
}
