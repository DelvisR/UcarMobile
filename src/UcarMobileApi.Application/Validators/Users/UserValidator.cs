using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Application.Validators.Users;

public abstract class UserValidatorBase<T> : AbstractValidator<T> where T : UserDto
{
    private readonly AppDbContext _dbContext;

    protected UserValidatorBase(AppDbContext dbContext)
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

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(50).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 50));

        RuleFor(u => u.Roles)
            .NotNull().WithMessage(ValidatorErrors.IsRequired)
            .Must(r => r.Any()).WithMessage(ValidatorErrors.NoEmpty);

        RuleForEach(u => u.Roles)
            .SetValidator(new RoleValidator(_dbContext));
    }

    private async Task<bool> BeUniqueEmail(UserDto dto, string email, CancellationToken ct)
    {
        return !await _dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.Email == email && u.Id != dto.Id, ct);
    }

    protected async Task<bool> BeUniqueCognitoId(UserDto dto, string cognitoId, CancellationToken ct)
    {
        return !await _dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.CognitoId == cognitoId && u.Id != dto.Id, ct);
    }
}

public class CreateUserValidator(AppDbContext dbContext) : UserValidatorBase<UserDto>(dbContext);

public class UpdateUserValidator : UserValidatorBase<UserDto>
{
    public UpdateUserValidator(AppDbContext dbContext) : base(dbContext)
    {
        RuleFor(x => x.CognitoId)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .MustAsync(BeUniqueCognitoId).WithMessage(ValidatorErrors.Duplicated);
    }
}

