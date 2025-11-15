using FluentValidation.TestHelper;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Validators.Payment;
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
        #region PaymentMethodCreateValidator Tests

        [Fact]
        public void PaymentMethodCreateValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var validator = new PaymentMethodCreateValidator();
            var dto = new PaymentMethodCreateDto(
                ClientId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
                Brand: "visa",
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
            output.WriteLine("Valid PaymentMethodCreateDto passed validation");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void PaymentMethodCreateValidator_InvalidClientId_FailsValidation(int clientId)
        {
            // Arrange
            var validator = new PaymentMethodCreateValidator();
            var dto = new PaymentMethodCreateDto(
                ClientId: clientId,
                ProviderPaymentMethodId: "pm_1234567890",
                Brand: "visa",
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ClientId);
            output.WriteLine($"Invalid ClientId {clientId} correctly failed validation");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void PaymentMethodCreateValidator_InvalidProviderPaymentMethodId_FailsValidation(string providerPaymentMethodId)
        {
            // Arrange
            var validator = new PaymentMethodCreateValidator();
            var dto = new PaymentMethodCreateDto(
                ClientId: 1,
                ProviderPaymentMethodId: providerPaymentMethodId,
                Brand: "visa",
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProviderPaymentMethodId);
            output.WriteLine($"Invalid ProviderPaymentMethodId '{providerPaymentMethodId}' correctly failed validation");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void PaymentMethodCreateValidator_InvalidBrand_FailsValidation(string brand)
        {
            // Arrange
            var validator = new PaymentMethodCreateValidator();
            var dto = new PaymentMethodCreateDto(
                ClientId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
                Brand: brand,
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Brand);
            output.WriteLine($"Invalid Brand '{brand}' correctly failed validation");
        }

        [Theory]
        [InlineData("123")]   // Too short
        [InlineData("12345")] // Too long
        [InlineData("")]      // Empty
        [InlineData(null)]    // Null
        public void PaymentMethodCreateValidator_InvalidLast4_FailsValidation(string last4)
        {
            // Arrange
            var validator = new PaymentMethodCreateValidator();
            var dto = new PaymentMethodCreateDto(
                ClientId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
                Brand: "visa",
                Last4: last4,
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Last4);
            output.WriteLine($"Invalid Last4 '{last4}' correctly failed validation");
        }

        [Theory]
        [InlineData(0)]   // Too low
        [InlineData(13)]  // Too high
        [InlineData(-1)]  // Negative
        public void PaymentMethodCreateValidator_InvalidExpMonth_FailsValidation(int expMonth)
        {
            // Arrange
            var validator = new PaymentMethodCreateValidator();
            var dto = new PaymentMethodCreateDto(
                ClientId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
                Brand: "visa",
                Last4: "4242",
                ExpMonth: expMonth,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ExpMonth);
            output.WriteLine($"Invalid ExpMonth {expMonth} correctly failed validation");
        }

        [Theory]
        [InlineData(2020)] // Past year
        [InlineData(2022)] // Past year
        public void PaymentMethodCreateValidator_InvalidExpYear_FailsValidation(int expYear)
        {
            // Arrange
            var validator = new PaymentMethodCreateValidator();
            var dto = new PaymentMethodCreateDto(
                ClientId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
                Brand: "visa",
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: expYear,
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ExpYear);
            output.WriteLine($"Invalid ExpYear {expYear} correctly failed validation");
        }

        #endregion

        #region CreatePaymentValidator Tests

        [Fact]
        public void CreatePaymentValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var validator = new CreatePaymentValidator();
            var dto = new PaymentCreateDto(
                ClientId: 1,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
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
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void CreatePaymentValidator_InvalidClientId_FailsValidation(int clientId)
        {
            // Arrange
            var validator = new CreatePaymentValidator();
            var dto = new PaymentCreateDto(
                ClientId: clientId,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
                IdempotencyKey: "payment:service123:client1:1634567890",
                AmountCents: 2500,
                Currency: "usd"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ClientId);
            output.WriteLine($"Invalid ClientId {clientId} correctly failed validation");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CreatePaymentValidator_InvalidProviderPaymentMethodId_FailsValidation(string providerPaymentMethodId)
        {
            // Arrange
            var validator = new CreatePaymentValidator();
            var dto = new PaymentCreateDto(
                ClientId: 1,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: providerPaymentMethodId,
                IdempotencyKey: "payment:service123:client1:1634567890",
                AmountCents: 2500,
                Currency: "usd"
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProviderPaymentMethodId);
            output.WriteLine($"Invalid ProviderPaymentMethodId '{providerPaymentMethodId}' correctly failed validation");
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
                ClientId: 1,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
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
                ClientId: 1,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: "pm_1234567890",
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
                ClientId: 1,
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
        [InlineData(0)]
        [InlineData(-1)]
        public void AttachPaymentMethodValidator_InvalidClientId_FailsValidation(int clientId)
        {
            // Arrange
            var validator = new AttachPaymentMethodValidator();
            var dto = new PaymentMethodAttachDto(
                ClientId: clientId,
                ProviderPaymentMethodId: "pm_1234567890",
                IsDefault: true
            );

            // Act
            var result = validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ClientId);
            output.WriteLine($"Invalid ClientId {clientId} correctly failed validation");
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
                ClientId: 1,
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
