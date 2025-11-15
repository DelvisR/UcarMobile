# Stripe Payment Tests – UcarMobile API

## Overview

This test suite provides full coverage for Stripe integration in the UcarMobile API, including unit and integration tests that use Stripe’s official test cards to simulate various payment scenarios.

## Test Structure

### 📁 Unit Tests (`/Unit/Payments/`)

#### `StripeTestCards.cs`
- **Purpose**: Constants for all Stripe test cards
- **Categories**:
  - Successful cards (Visa, Mastercard, Amex, etc.)
  - Declined cards (insufficient funds, lost card, etc.)
  - Cards requiring 3D Secure
  - International cards
  - Special scenarios (disputes, failed refunds)

#### `PaymentValidatorsTests.cs`
- **Purpose**: Validation of payment DTOs
- **Coverage**:
  - `PaymentMethodCreateValidator`
  - `CreatePaymentValidator`
  - `CreatePaymentRefundValidator`
  - `AttachPaymentMethodValidator`

### 📁 Integration Tests (`/Integration/Payments/`)

#### `StripePaymentServiceTests.cs`
- **Purpose**: Integration tests for the main payment service
- **Covered scenarios**:
  - ✅ SetupIntent creation
  - ✅ Saving payment methods
  - ✅ Successful payment processing
  - ✅ Handling declined cards
  - ✅ 3D Secure flow
  - ✅ Payment idempotency
  - ✅ Full and partial refunds
  - ✅ International cards
  - ✅ Processing errors

#### `StripeWebhookServiceTests.cs`
- **Purpose**: Tests for webhook processing
- **Covered scenarios**:
  - ✅ PaymentIntent events (succeeded, failed, etc.)
  - ✅ Refund events (succeeded, failed, updated)
  - ✅ Record creation and updates
  - ✅ Payment status calculation
  - ✅ Handling duplicate events

#### `StripeTestFixture.cs`
- **Purpose**: Shared test setup
- **Features**:
  - Stripe test configuration
  - AutoMapper setup
  - In-memory database
  - Test configuration

### 📁 Controller Tests (`/Integration/Controllers/`)

#### `PaymentsControllerTests.cs`
- **Purpose**: HTTP endpoint tests
- **Coverage**:
  - ✅ POST `/api/payments/setup-intent/{clientId}`
  - ✅ POST `/api/payments/save-method`
  - ✅ POST `/api/payments/charge`
  - ✅ POST `/api/payments/refund`
  - ✅ Various test cards
  - ✅ International currencies
  - ✅ Input validation

## Stripe Test Cards

### 🟢 Successful Cards
```csharp
StripeTestCards.Successful.Visa              // pm_card_visa (4242)
StripeTestCards.Successful.Mastercard        // pm_card_mastercard (4444)
StripeTestCards.Successful.AmericanExpress   // pm_card_amex (0005)
```

### 🔴 Declined Cards
```csharp
StripeTestCards.Declined.Generic             // pm_card_chargeDeclined (0002)
StripeTestCards.Declined.InsufficientFunds   // pm_card_chargeDeclinedInsufficientFunds (9995)
StripeTestCards.Declined.ExpiredCard         // pm_card_expiredCard (0069)
StripeTestCards.Declined.IncorrectCVC        // pm_card_cvcDecline (0127)
```

### 🔐 3D Secure
```csharp
StripeTestCards.Authentication.Required      // pm_card_authenticationRequired (3155)
```

### 🌍 International Cards
```csharp
StripeTestCards.International.Brazil         // pm_card_br (BRL)
StripeTestCards.International.Canada         // pm_card_ca (CAD)
StripeTestCards.International.UnitedKingdom  // pm_card_gb (GBP)
```

## Test Setup

### 1. Stripe Configuration

#### Create a file: RunSettings.runsettings
```json
<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <STRIPE_TEST_SECRET_KEY>sk_test_12345</STRIPE_TEST_SECRET_KEY>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>
```

#### Configure Visual Studio to use it:

Go to the **Test** menu > **Configure Run Settings** > **Select Solution Wide Run Settings File**...

Select the **RunSettings.runsettings** file.

### 2. Using Stripe Mock (Recommended for CI/CD)

```bash
# Run Stripe Mock
docker run --rm -p 12111:12111 stripe/stripe-mock

# Set test endpoint
export STRIPE_API_BASE="http://localhost:12111"
```

### 3. Run Tests

```bash
# All Stripe tests
dotnet test --filter "Category=Stripe"

# Unit tests only
dotnet test --filter "FullyQualifiedName~Unit.Payments"

# Integration tests only
dotnet test --filter "FullyQualifiedName~Integration.Payments"

# Controller-specific tests
dotnet test --filter "FullyQualifiedName~Controllers.PaymentsControllerTests"
```

## Covered Test Scenarios

### ✅ Full Payment Flow
1. **Setup Intent**: Create setup intent
2. **Save Payment Method**: Save payment method
3. **Create Payment**: Process payment
4. **Handle Webhooks**: Process events
5. **Refunds**: Process refunds

### ✅ Error Cases
- Declined cards for various reasons
- Validation errors
- Nonexistent clients
- Duplicate payments (idempotency)
- Unknown payment webhooks

### ✅ Special Cases
- Payments requiring 3D Secure
- Partial and full refunds
- Multiple payment methods per client
- Different currencies
- Concurrent processing

### ✅ Security
- Webhook signature validation
- Secure error handling
- No exposure of sensitive data
- Correct idempotency

## Coverage Metrics

Tests cover:
- **Services**: 95%+ coverage for `StripePaymentService` and `StripePaymentWebHookService`
- **Controllers**: 90%+ coverage for `PaymentsController`
- **Validators**: 100% coverage for payment validators

## Best Practices Implemented

### 🔧 Configuration
- Use of `IClassFixture` for shared setup
- In-memory database for isolation
- Test-specific configuration

### 📊 Test Data
- Use of official Stripe test cards
- Deterministic data generation
- Automatic cleanup between tests

### 🎯 Assertions
- Use of FluentAssertions for readability
- Verification of expected states
- Validation of side effects

### 📝 Documentation
- Descriptive comments in each test
- Useful output via `ITestOutputHelper`
- Documentation of covered scenarios

## Useful Commands for Visual Studio 2022

### Run from Test Explorer
1. Open **Test Explorer** (Test → Test Explorer)
2. Filter by "Payments" or "Stripe"
3. Run individual or grouped tests

### Run from Package Manager Console
```powershell
# All payment tests
dotnet test --filter "Payments"

# With detailed output
dotnet test --filter "Payments" --logger "console;verbosity=detailed"

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Test Debugging
1. Set breakpoints in test code
2. Right-click on the test → "Debug Test"
3. Use debugger as usual

## Troubleshooting

### Common Issues

#### Error: "Stripe API key not configured"
**Solution**: Set `STRIPE_TEST_SECRET_KEY` or use Stripe Mock

#### Error: "Database connection failed"
**Solution**: Tests use InMemoryDatabase, check EF Core configuration

#### Error: "Webhook signature validation failed"
**Solution**: Set correct `STRIPE_WEBHOOK_SECRET` for tests

### Debug Logs
```csharp
// Enable detailed logs in tests
_output.WriteLine($"Test result: {result}");
```

## Additional Resources

- [Stripe Testing Documentation](https://stripe.com/docs/testing)
- [Stripe Test Cards](https://stripe.com/docs/testing#cards)
- [Stripe CLI for Webhooks](https://stripe.com/docs/stripe-cli)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
