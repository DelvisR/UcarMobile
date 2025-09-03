using FluentValidation;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.Validators.Users;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.CognitoId)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));
    }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));
    }
}