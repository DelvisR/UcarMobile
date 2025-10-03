using FluentValidation;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Validators.Users;

namespace UcarMobileApi.Application.Validators;

public class ClientValidator : UserValidator<ClientDto>
{
    public ClientValidator(IAppDbContext dbContext) : base(dbContext)
    {
        RuleFor(c => c.Address)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256));
    }
}
