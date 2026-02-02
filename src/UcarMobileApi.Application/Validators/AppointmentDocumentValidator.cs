using FluentValidation;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators;

public class AddAppointmentDocumentsDtoValidator : AbstractValidator<AddAppointmentDocumentsDto>
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public AddAppointmentDocumentsDtoValidator()
    {
        // Validate that at least one file is provided
        RuleFor(x => x.Files)
            .NotEmpty().WithMessage("At least one file must be uploaded.");

        // Validate size of each file
        RuleForEach(x => x.Files)
            .Must(file => file.Length <= MaxFileSizeBytes)
            .WithMessage($"Each file must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB.");

        // Optional: Validate Prefix length
        RuleFor(x => x.Prefix)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50))
            .When(x => !string.IsNullOrEmpty(x.Prefix));

        // Validate Source enum
        RuleFor(x => x.Source)
            .IsInEnum().WithMessage(ValidatorErrors.InvalidValue);
    }
}
