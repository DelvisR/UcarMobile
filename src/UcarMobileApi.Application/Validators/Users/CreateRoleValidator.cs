using FluentValidation;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.Validators.Users;

public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
                .MaximumLength(100).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 100));

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}