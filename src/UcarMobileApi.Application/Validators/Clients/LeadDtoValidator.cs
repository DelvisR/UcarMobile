using FluentValidation;
using UcarMobileApi.Application.DTOs.Clients;

namespace UcarMobileApi.Application.Validators.Clients;

/// <summary>
/// Validator for LeadDto using FluentValidation.
/// </summary>
public class LeadValidator : AbstractValidator<LeadDto>
{
    public LeadValidator()
    {
        RuleFor(u => u.Id)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero)
            .When(u => u.Id != 0);

        // Phone: required, fixed length with regex
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .Matches(@"^\d{10}$").WithMessage("Phone must be exactly 10 digits");

        // FirstName: required, length limits
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));

        // LastName: required, length limits
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));
    }
}
