using System;
using System.Linq;
using FluentValidation;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Technicians;

/// <summary>
/// Validator for AvailableSlotRequestDto
/// </summary>
public class AvailableSlotRequestValidator : AbstractValidator<AvailableSlotRequestDto>
{
    public AvailableSlotRequestValidator()
    {
        // Validate Latitude
        RuleFor(x => x.Lat)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        // Validate Longitude
        RuleFor(x => x.Lng)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        // Validate that coordinates are not exactly 0,0 (Null Island - likely an error)
        RuleFor(x => x)
            .Must(x => x.Lat != 0 || x.Lng != 0)
            .WithMessage("Invalid coordinates: Latitude and Longitude cannot both be zero.")
            .When(x => x.Lat == 0 && x.Lng == 0);

        // Validate ZipCode
        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage(ValidatorErrors.IsRequired)
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("ZipCode must be in valid US format (12345 or 12345-6789).");

        RuleFor(x => x.Specialties)
            .NotEmpty()
            .WithMessage("At least one service category is required.")
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("All service category IDs must be positive integers.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Service category IDs must be unique.");
    }
}

public class NearestAvailableRequestValidator
    : AbstractValidator<NearestAvailableRequestDto>
{
    public NearestAvailableRequestValidator()
    {
        // Reutiliza TODAS las reglas del padre
        Include(new AvailableSlotRequestValidator());

        // Reglas propias del DTO hijo
        RuleFor(x => x.LocalStar)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired);

        RuleFor(x => x.LocalEnd)
            .NotEmpty().WithMessage(ValidatorErrors.IsRequired)
            .GreaterThan(x => x.LocalStar)
            .WithMessage("LocalEnd must be greater than LocalStar.");

        // Opcional recomendado: evitar DateTime sin Kind
        RuleFor(x => x.LocalStar)
            .Must(d => d.Kind != DateTimeKind.Unspecified)
            .WithMessage("LocalStar must include timezone info.");

        RuleFor(x => x.LocalEnd)
            .Must(d => d.Kind != DateTimeKind.Unspecified)
            .WithMessage("EndLocal must include timezone info.");
    }
}


