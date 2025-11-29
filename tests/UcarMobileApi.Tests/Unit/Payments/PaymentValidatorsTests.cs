using FluentValidation.TestHelper;
using UcarMobileApi.Application.DTOs.Payments;
using UcarMobileApi.Application.Validators;
using Xunit;
using Xunit.Abstractions;

namespace UcarMobileApi.Tests.Unit.Payments
{
    /// <summary>
    /// Unit tests for payment validators
    /// Tests validation rules for payment DTOs
    /// </summary>
    public class PaymentValidatorsTests(ITestOutputHelper output)
    {
        #region CreatePaymentValidator Tests

        [Fact]
        public void CreatePaymentValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var validator = new CreatePaymentValidator();
            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: "payment:service123:client1:1634567890",
                AmountCents: 2500,
                Currency: "usd"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
            output.WriteLine("Valid PaymentCreateDto passed validation");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CreatePaymentValidator_InvalidIdempotencyKey_FailsValidation(string idempotencyKey)
        {
            // Arrange
            var validator = new CreatePaymentValidator();
            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: idempotencyKey,
                AmountCents: 2500,
                Currency: "usd"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.IdempotencyKey);
            output.WriteLine($"Invalid IdempotencyKey '{idempotencyKey}' correctly failed validation");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void CreatePaymentValidator_InvalidAmountCents_FailsValidation(long amountCents)
        {
            // Arrange
            var validator = new CreatePaymentValidator();
            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: "payment:service123:client1:1634567890",
                AmountCents: amountCents,
                Currency: "usd"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AmountCents);
            output.WriteLine($"Invalid AmountCents {amountCents} correctly failed validation");
        }

        #endregion

        #region CreatePaymentRefundValidator Tests

        [Fact]
        public void CreatePaymentRefundValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var validator = new CreatePaymentRefundValidator();
            var dto = new PaymentRefundDto(
                PaymentId: 1,
                AmountCents: 1000,
                IdempotencyKey: "refund:payment1:1634567890"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
            output.WriteLine("Valid PaymentRefundDto passed validation");
        }

        [Fact]
        public void CreatePaymentRefundValidator_ValidDtoWithNullAmount_PassesValidation()
        {
            // Arrange
            var validator = new CreatePaymentRefundValidator();
            var dto = new PaymentRefundDto(
                PaymentId: 1,
                AmountCents: null, // Full refund
                IdempotencyKey: "refund:payment1:1634567890"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
            output.WriteLine("Valid PaymentRefundDto with null amount (full refund) passed validation");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void CreatePaymentRefundValidator_InvalidPaymentId_FailsValidation(int paymentId)
        {
            // Arrange
            var validator = new CreatePaymentRefundValidator();
            var dto = new PaymentRefundDto(
                PaymentId: paymentId,
                AmountCents: 1000,
                IdempotencyKey: "refund:payment1:1634567890"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PaymentId);
            output.WriteLine($"Invalid PaymentId {paymentId} correctly failed validation");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CreatePaymentRefundValidator_InvalidIdempotencyKey_FailsValidation(string idempotencyKey)
        {
            // Arrange
            var validator = new CreatePaymentRefundValidator();
            var dto = new PaymentRefundDto(
                PaymentId: 1,
                AmountCents: 1000,
                IdempotencyKey: idempotencyKey
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.IdempotencyKey);
            output.WriteLine($"Invalid IdempotencyKey '{idempotencyKey}' correctly failed validation");
        }

        #endregion

        #region AttachPaymentMethodValidator Tests

        [Fact]
        public void AttachPaymentMethodValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var validator = new AttachPaymentMethodValidator();
            var dto = new PaymentMethodAttachDto(
                ProviderPaymentMethodId: "pm_1234567890",
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
            output.WriteLine("Valid PaymentMethodAttachDto passed validation");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void AttachPaymentMethodValidator_InvalidProviderPaymentMethodId_FailsValidation(string providerPaymentMethodId)
        {
            // Arrange
            var validator = new AttachPaymentMethodValidator();
            var dto = new PaymentMethodAttachDto(
                ProviderPaymentMethodId: providerPaymentMethodId,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProviderPaymentMethodId);
            output.WriteLine($"Invalid ProviderPaymentMethodId '{providerPaymentMethodId}' correctly failed validation");
        }

        #endregion
    }
}
