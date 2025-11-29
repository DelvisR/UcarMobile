using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Common;
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
    }

    private async Task<bool> BeUniqueEmail(UserDto dto, string email, CancellationToken ct)
    {
        return !await _dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.Email == email && u.Id != dto.Id, ct);
    }
}

/// <summary>
/// Generic validator for any DTO that inherits from UserAccountDto.
/// Includes role validation in addition to base user validation.
/// </summary>
/// <typeparam name="TUserAccount">Type that inherits from UserAccountDto</typeparam>
public abstract class UserAccountValidator<TUserAccount> : UserValidator<TUserAccount>
    where TUserAccount : UserAccountDto
{
    protected UserAccountValidator(IAppDbContext dbContext) : base(dbContext)
    {
        // Role validation - common for all UserAccount-derived DTOs
        RuleFor(u => u.Roles)
            .Must(r => r == null || r.Count > 0)
            .WithMessage(ValidatorErrors.NoEmpty);

        RuleForEach(u => u.Roles)
            .SetValidator(new UserRoleValidator());
    }
}

/// <summary>
/// Concrete validator for UserAccountDto.
/// </summary>
public class UserAccountValidator(IAppDbContext dbContext) : UserAccountValidator<UserAccountDto>(dbContext)
{
    // No additional validation needed for base UserAccountDto
}

/// <summary>
/// Validator for a list of RoleDto elements.
/// Ensures unique roles and valid IDs.
/// </summary>
public class UserRoleListValidator : AbstractValidator<List<RoleDto>>
{
    public UserRoleListValidator()
    {
        // Must have at least one role (if list is provided)
        RuleFor(list => list)
            .NotEmpty().WithMessage(ValidatorErrors.NoEmpty);

        // Must not contain duplicates based on Role.Id
        RuleFor(list => list)
            .MustHaveUnique(r => r.Id)
            .When(list => list.Count != 0)
            .WithMessage("Duplicate roles are not allowed.");

        // Validate each RoleDto element
        RuleForEach(list => list)
            .SetValidator(new UserRoleValidator());
    }
}

/// <summary>
/// Validator for a single RoleDto element.
/// </summary>
public class UserRoleValidator : AbstractValidator<RoleDto>
{
    public UserRoleValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(0)
            .WithMessage(ValidatorErrors.GreaterThanZero);
    }
}
