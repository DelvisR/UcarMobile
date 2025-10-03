using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Validators.Users;

public class UserValidator<TUser> : AbstractValidator<TUser> where TUser : UserDto
{
    private readonly IAppDbContext _dbContext;

    public UserValidator(IAppDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(u => u.Id)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero)
            .When(u => u.Id != 0);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .EmailAddress()
            .MustAsync(BeUniqueEmail).WithMessage(ValidatorErrors.Duplicated);

        RuleFor(u => u.Phone)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .Matches(@"^\d{10}$").WithMessage("Phone must be exactly 10 digits");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));

        RuleFor(x => x.AuthProviderId)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .MustAsync(BeUniqueAuthProviderId).WithMessage(ValidatorErrors.Duplicated);

        RuleFor(u => u.Roles)
            .Must(r => r == null || r.Count > 0)
            .WithMessage(ValidatorErrors.NoEmpty);

        RuleForEach(u => u.Roles)
            .SetValidator(new RoleValidator(_dbContext));
    }

    private async Task<bool> BeUniqueEmail(UserDto dto, string email, CancellationToken ct)
    {
        return !await _dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.Email == email && u.Id != dto.Id, ct);
    }

    protected async Task<bool> BeUniqueAuthProviderId(UserDto dto, string authProviderId, CancellationToken ct)
    {
        return !await _dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.AuthProviderId == authProviderId && u.Id != dto.Id, ct);
    }
}

