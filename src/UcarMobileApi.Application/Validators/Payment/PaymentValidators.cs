using System;
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
        RuleFor(x => x.ClientId).GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
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
/// Validates <see cref="PaymentMethodCreateDto"/> requests.
/// </summary>
public class PaymentMethodCreateValidator : AbstractValidator<PaymentMethodCreateDto>
{
    public PaymentMethodCreateValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
        RuleFor(x => x.ProviderPaymentMethodId).NotEmpty().WithMessage(ValidatorErrors.IsRequired).MaximumLength(64).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 64));
        RuleFor(x => x.Brand).NotEmpty().WithMessage(ValidatorErrors.IsRequired).MaximumLength(32).WithMessage(string.Format(ValidatorErrors.MaxLengthExceeded, 32));
        RuleFor(x => x.Last4).NotEmpty().WithMessage(ValidatorErrors.IsRequired).Length(4).WithMessage(string.Format(ValidatorErrors.LengthMismatch, 4));
        RuleFor(x => x.ExpMonth).InclusiveBetween(1, 12).WithMessage(string.Format(ValidatorErrors.Between, 1, 12));
        RuleFor(x => x.ExpYear).GreaterThan(DateTime.UtcNow.Year - 1).WithMessage(string.Format(ValidatorErrors.GreaterThan, DateTime.UtcNow.Year - 1));
    }
}

/// <summary>
/// Validates <see cref="PaymentMethodAttachDto"/> requests.
/// </summary>
public class AttachPaymentMethodValidator : AbstractValidator<PaymentMethodAttachDto>
{
    public AttachPaymentMethodValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0).WithMessage(ValidatorErrors.GreaterThanZero);
        RuleFor(x => x.ProviderPaymentMethodId).NotEmpty().WithMessage(ValidatorErrors.IsRequired);
    }
}
