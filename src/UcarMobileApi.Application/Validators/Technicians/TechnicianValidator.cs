using System.Collections.Generic;
using FluentValidation;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Validators.Common;
using UcarMobileApi.Application.Validators.Users;

namespace UcarMobileApi.Application.Validators.Technicians;

/// <summary>
/// Validator for TechnicianDto.
/// Includes validation rules for service zones, specialities,
/// and inherits base user account validation.
/// </summary>
public class TechnicianValidator : UserAccountValidator<TechnicianDto>
{
    public TechnicianValidator(IAppDbContext dbContext) : base(dbContext)
    {
        RuleFor(a => a.BaseAddress)
            .SetValidator(new AddressInfoValidator());

        // --- SERVICE ZONES ---------------------------------------------------

        RuleFor(t => t.ServiceZones)
            .MustHaveSingle(z => z.IsPrimaryZone)
            .When(t => t.ServiceZones.Count != 0)
            .WithMessage("Exactly one service zone must be marked as primary when zones are assigned.");

        RuleFor(t => t.ServiceZones)
            .MustHaveUnique(z => z.ServiceZone!.Id)
            .When(t => t.ServiceZones.Count != 0)
            .WithMessage("Duplicate service zones are not allowed.");

        RuleForEach(t => t.ServiceZones)
            .SetValidator(new TechnicianServiceZoneValidator());

        // --- SPECIALITIES -----------------------------------------------------

        RuleFor(t => t.Specialities)
            .MustHaveUnique(s => s.ServiceCategory!.Id)
            .When(t => t.Specialities.Count != 0)
            .WithMessage("Duplicate specialities are not allowed.");

        RuleForEach(t => t.Specialities)
            .SetValidator(new TechnicianSpecialityValidator());
    }
}

/// <summary>
/// Validator for a list of TechnicianServiceZoneDto items.
/// Makes use of reusable collection extension rules.
/// </summary>
public class TechnicianServiceZoneListValidator : AbstractValidator<List<TechnicianServiceZoneDto>>
{
    public TechnicianServiceZoneListValidator()
    {
        RuleFor(list => list)
            .MustHaveSingle(z => z.IsPrimaryZone)
            .When(list => list.Count != 0)
            .WithMessage("Exactly one service zone must be marked as primary when zones are assigned.");

        RuleFor(list => list)
            .MustHaveUnique(z => z.ServiceZone!.Id)
            .When(list => list.Count != 0)
            .WithMessage("Duplicate service zones are not allowed.");

        RuleForEach(list => list)
            .SetValidator(new TechnicianServiceZoneValidator());
    }
}

/// <summary>
/// Validator for TechnicianServiceZoneDto elements.
/// </summary>
public class TechnicianServiceZoneValidator : AbstractValidator<TechnicianServiceZoneDto>
{
    public TechnicianServiceZoneValidator()
    {
        RuleFor(z => z.ServiceZone)
            .NotNull().WithMessage(ValidatorErrors.IsRequired);

        RuleFor(z => z.ServiceZone!.Id)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
    }
}

/// <summary>
/// Validator for a list of TechnicianSpecialityDto.
/// Uses uniqueness validation from collection extensions.
/// </summary>
public class TechnicianSpecialityListValidator : AbstractValidator<List<TechnicianSpecialityDto>>
{
    public TechnicianSpecialityListValidator()
    {
        RuleFor(list => list)
            .MustHaveUnique(s => s.ServiceCategory!.Id)
            .When(list => list.Count != 0)
            .WithMessage("Duplicate specialities are not allowed.");

        RuleForEach(list => list)
            .SetValidator(new TechnicianSpecialityValidator());
    }
}

/// <summary>
/// Validator for TechnicianSpecialityDto elements.
/// </summary>
public class TechnicianSpecialityValidator : AbstractValidator<TechnicianSpecialityDto>
{
    public TechnicianSpecialityValidator()
    {
        RuleFor(s => s.ServiceCategory)
            .NotNull().WithMessage(ValidatorErrors.IsRequired);

        RuleFor(s => s.ServiceCategory!.Id)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);

        RuleFor(s => s.SkillLevel)
            .InclusiveBetween((byte)1, (byte)4)
            .WithMessage("Skill level must be between 1 (Junior) and 4 (Master).");
    }
}
