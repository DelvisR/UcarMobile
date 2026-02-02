using FluentValidation;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Application.Validators.Clients;

public abstract class ClientVehicleBaseValidator<T> : AbstractValidator<T> where T : ClientVehicleBaseDto
{
    protected ClientVehicleBaseValidator()
    {
        RuleFor(x => x.VIN)
            .MaximumLength(17).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 17))
            .Matches("^[A-HJ-NPR-Z0-9]{17}$").WithMessage(ValidatorErrors.InvalidValue)
            .When(x => !string.IsNullOrWhiteSpace(x.VIN));

        RuleFor(x => x.LicensePlate)
            .MaximumLength(20).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 20))
            .When(x => !string.IsNullOrWhiteSpace(x.LicensePlate));

        RuleFor(x => x.Submodel)
            .MaximumLength(60).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 60))
            .When(x => !string.IsNullOrWhiteSpace(x.Submodel));

        RuleFor(x => x.Engine)
            .MaximumLength(80).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 80))
            .When(x => !string.IsNullOrWhiteSpace(x.Engine));

        RuleFor(x => x.VehicleType)
            .MaximumLength(60).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 60))
            .When(x => !string.IsNullOrWhiteSpace(x.VehicleType));

        RuleFor(x => x.BodyType)
            .MaximumLength(80).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 80))
            .When(x => !string.IsNullOrWhiteSpace(x.BodyType));

        RuleFor(x => x.Fuel)
            .MaximumLength(60).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 60))
            .When(x => !string.IsNullOrWhiteSpace(x.Fuel));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 1000))
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.AzId)
            .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative)
            .When(x => x.AzId.HasValue);
    }
}

public class ClientVehicleUpsertDtoValidator : ClientVehicleBaseValidator<ClientVehicleUpsertDto>
{
    public ClientVehicleUpsertDtoValidator()
    {
        RuleFor(x => x.VehicleId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
    }
}

public class ClientVehicleUpdateDtoValidator : ClientVehicleBaseValidator<ClientVehicleUpdateDto>
{
    public ClientVehicleUpdateDtoValidator()
    {
        RuleFor(x => x.VehicleId)
            .GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero)
            .When(x => x.VehicleId.HasValue);

        RuleFor(x => x.OdometerKm)
            .GreaterThanOrEqualTo(0).WithMessage(ValidatorErrors.MustBeNonNegative)
            .When(x => x.OdometerKm.HasValue);
    }
}

