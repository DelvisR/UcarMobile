using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Appointments;

public class AppointmentNoteCreateDtoValidator : AbstractValidator<AppointmentNoteBaseDto>
{
    public AppointmentNoteCreateDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired);
        RuleFor(x => x.Source)
            .IsInEnum().WithMessage(ValidatorErrors.InvalidValue);
    }
}
