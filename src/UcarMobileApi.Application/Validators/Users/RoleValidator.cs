using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Common;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Validators.Users;

public class RoleValidator : AbstractValidator<RoleDto>
{
    private readonly IAppDbContext _dbContext;

    public RoleValidator(IAppDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
                .MaximumLength(100).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 100))
                .MustAsync(BeUniqueName).WithMessage(ValidatorErrors.Duplicated);

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    private async Task<bool> BeUniqueName(RoleDto dto, string name, CancellationToken ct)
    {
        return !await _dbContext.Set<Role>()
            .AsNoTracking()
            .AnyAsync(r => r.Name == name && r.Id != dto.Id, ct);
    }
}
