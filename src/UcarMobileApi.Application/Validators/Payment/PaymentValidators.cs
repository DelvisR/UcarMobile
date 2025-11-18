using FluentValidation;
using UcarMobileApi.Application.DTOs;

namespace UcarMobileApi.Application.Validators.Payment;

/// <summary>
/// Validates <see cref="PaymentCreateDto"/> requests.
/// </summary>
public class CreatePaymentValidator : AbstractValidator<PaymentCreateDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.PaymentMethodId).GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
        RuleFor(x => x.ProviderPaymentMethodId).NotEmpty().WithMessage(ValidatorErrors.IsRequired);
        RuleFor(x => x.IdempotencyKey).NotEmpty().WithMessage(ValidatorErrors.IsRequired);
        RuleFor(x => x.AmountCents).GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
    }
}

/// <summary>
/// Validates <see cref="PaymentRefundDto"/> requests.
/// </summary>
public class CreatePaymentRefundValidator : AbstractValidator<PaymentRefundDto>
{
    public CreatePaymentRefundValidator()
    {
        RuleFor(x => x.PaymentId).GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
        RuleFor(x => x.IdempotencyKey).NotEmpty().WithMessage(ValidatorErrors.IsRequired);
    }
}

/// <summary>
/// Validates <see cref="PaymentMethodAttachDto"/> requests.
/// </summary>
public class AttachPaymentMethodValidator : AbstractValidator<PaymentMethodAttachDto>
{
    public AttachPaymentMethodValidator()
    {
        RuleFor(x => x.ProviderPaymentMethodId).NotEmpty().WithMessage(ValidatorErrors.IsRequired);
    }
}
