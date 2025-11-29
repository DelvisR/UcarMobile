using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Common;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Validators.Clients;

public class ClientValidator : UserValidator<ClientDto>
{
    private readonly IAppDbContext _dbContext;

    public ClientValidator(IAppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.AuthProviderId)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256))
            .MustAsync(BeUniqueAuthProviderId).WithMessage(ValidatorErrors.Duplicated)
            .When(u => u.Id == 0);

        RuleFor(c => c.Address)
            .MaximumLength(256).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 256));
    }

    protected async Task<bool> BeUniqueAuthProviderId(UserDto dto, string authProviderId, CancellationToken ct)
    {
        return !await _dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(u => u.AuthProviderId == authProviderId && u.Id != dto.Id, ct);
    }
}
