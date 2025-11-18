using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Application.Validators.Payment;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Infrastructure.Data;
using PaymentMethod = UcarMobileApi.Core.Entities.Payments.PaymentMethod;

namespace UcarMobileApi.Infrastructure.Services.Payments;

/// <summary>
/// Stripe-based implementation of <see cref="IPaymentService"/>.
/// Manages setup intents, payment methods, charges, and refunds using Stripe API.
/// </summary>
public class StripePaymentService(AppDbContext context, IMapper mapper, ILogger<StripePaymentService> logger, ICacheService cache) : IPaymentService
{
    private readonly CustomerService _customerService = new();
    private readonly SetupIntentService _setupIntentService = new();
    private readonly PaymentMethodService _paymentMethodService = new();
    private readonly PaymentIntentService _paymentIntentService = new();
    private readonly RefundService _refundService = new();

    private static readonly TimeSpan IdempotencyTtl = TimeSpan.FromMinutes(15);

    #region Helper methods

    private async Task<string> CreateOrGetCustomerAsync(string authProviderId, CancellationToken cancellationToken = default)
    {
        var client = await context.Set<Client>()
            .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Client with AuthProviderId {authProviderId} not found.");

        return await CreateOrGetCustomerAsync(client, cancellationToken);
    }

    private async Task<string> CreateOrGetCustomerAsync(Client client, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(client.ProviderPaymentCustomerId))
            return client.ProviderPaymentCustomerId;

        var created = await _customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = client.Email,
            Name = $"{client.FirstName} {client.LastName}",
            Phone = client.Phone
        }, cancellationToken: cancellationToken);

        client.ProviderPaymentCustomerId = created.Id;

        await context.SaveChangesAsync(cancellationToken);

        return created.Id;
    }

    private async Task<T> HandleStripeOperationAsync<T>(Func<Task<T>> action, string operationName)
    {
        try
        {
            return await action();
        }
        catch (StripeException ex)
        {
            logger.LogWarning("StripeException in {Operation}: Type={Type}, Code={Code}, DeclineCode={DeclineCode}, RequestId={RequestId}, Message={Message}",
                operationName,
                ex.StripeError?.Type,
                ex.StripeError?.Code,
                ex.StripeError?.DeclineCode,
                ex.StripeResponse?.RequestId,
                ex.StripeError?.Message ?? ex.Message);

            // encapsulamos el código de error para propagarlo de forma controlada
            throw new ApplicationException($"stripe_error:{ex.StripeError?.DeclineCode ?? ex.StripeError?.Code ?? "unknown"}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in {Operation}", operationName);
            throw new ApplicationException("internal_error");
        }
    }

    #endregion

    public async Task<PaymentSetupDto> CreateSetupIntentAsync(string authProviderId, CancellationToken cancellationToken = default)
    {
        var customerId = await CreateOrGetCustomerAsync(authProviderId, cancellationToken);

        var setup = await HandleStripeOperationAsync(
            () => _setupIntentService.CreateAsync(new SetupIntentCreateOptions
            {
                Customer = customerId,
                Usage = "off_session"
            }, null, cancellationToken),
            "CreateSetupIntent"
        );

        return new PaymentSetupDto(setup.ClientSecret, customerId);
    }

    public async Task<PaymentMethodDto> AttachPaymentMethodAsync(string authProviderId, PaymentMethodAttachDto dto, CancellationToken cancellationToken = default)
    {
        var validator = new AttachPaymentMethodValidator();
        await validator.ValidateAndThrowAsync(dto, cancellationToken);

        var client = await context.Set<Client>().FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Client with AuthProviderId {authProviderId} not found.");

        var customerId = client.ProviderPaymentCustomerId
                         ?? await CreateOrGetCustomerAsync(client, cancellationToken);

        var pm = await HandleStripeOperationAsync(
            () => _paymentMethodService.AttachAsync(dto.ProviderPaymentMethodId,
                new PaymentMethodAttachOptions { Customer = customerId },
                null, cancellationToken),
            "AttachPaymentMethod"
        );

        var entity = mapper.Map<PaymentMethod>(pm.Card);
        entity.ClientId = client.Id;
        entity.ProviderPaymentMethodId = pm.Id;
        entity.IsDefault = dto.IsDefault;

        if (dto.IsDefault)
        {
            var others = await context.Set<PaymentMethod>()
                .Where(x => x.ClientId == client.Id && x.Id != entity.Id)
                .ToListAsync(cancellationToken);
            foreach (var other in others)
                other.IsDefault = false;
        }

        context.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<PaymentMethodDto>(entity);
    }

    /// <summary>
    /// Creates and confirms a payment for the specified client using the selected payment method.
    /// Handles Stripe response codes and returns only structured information (no user messages).
    /// </summary>
    /// <param name="authProviderId">Current user Auth Provider Id</param>
    /// <param name="dto">The DTO containing client ID, payment method ID, amount, and currency.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="PaymentDto"/> representing the result of the payment attempt, including
    /// provider status, client secret (for 3D Secure), and optional error codes.
    /// </returns>
    public async Task<PaymentDto> CreatePaymentAsync(string authProviderId, PaymentCreateDto dto, CancellationToken cancellationToken = default)
    {
        var validator = new CreatePaymentValidator();
        await validator.ValidateAndThrowAsync(dto, cancellationToken);

        // Verify local idempotence
        var existingPaymentId = await cache.GetAsync<int?>(dto.IdempotencyKey);
        if (existingPaymentId.HasValue)
        {
            var existing = await context.Set<Payment>().FindAsync([existingPaymentId], cancellationToken);
            if (existing != null)
                return mapper.Map<PaymentDto>(existing);
        }

        var client = await context.Set<Client>().FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Client with AuthProviderId {authProviderId} not found.");

        if (string.IsNullOrEmpty(client.ProviderPaymentCustomerId))
            throw new InvalidOperationException("Client does not have an associated Stripe customer ID.");

        // Generate RequestOptions with idempotency
        var requestOptions = new RequestOptions { IdempotencyKey = dto.IdempotencyKey };

        var options = new PaymentIntentCreateOptions
        {
            Customer = client.ProviderPaymentCustomerId,
            PaymentMethod = dto.ProviderPaymentMethodId,
            Amount = dto.AmountCents,
            Currency = dto.Currency,
            OffSession = true,
            Confirm = true,
            Metadata = new Dictionary<string, string>
            {
                { "clientId", client.Id.ToString() },
                { "source", "UcarMobile" },
                { "idempotencyKey", requestOptions.IdempotencyKey }
            }
        };

        try
        {
            var pi = await HandleStripeOperationAsync(
                () => _paymentIntentService.CreateAsync(options, requestOptions, cancellationToken),
                "CreatePaymentIntent"
            );

            string? errorCode = null;
            if (pi.Status == "requires_payment_method" && pi.LastPaymentError != null)
            {
                errorCode = pi.LastPaymentError.Code ?? "payment_failed";
            }

            var entity = mapper.Map<Payment>(pi);
            entity.ClientId = client.Id;
            entity.PaymentMethodId = dto.PaymentMethodId;

            await context.Set<Payment>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Save to local cache
            if (!string.IsNullOrEmpty(requestOptions.IdempotencyKey))
                await cache.SetAsync(requestOptions.IdempotencyKey, entity.Id, ttl: IdempotencyTtl, slidingExpiration: null);

            return new PaymentDto(pi.Id, pi.Status, pi.Amount, pi.Currency)
            {
                ClientSecret = pi.ClientSecret,
                ErrorCode = errorCode
            };
        }
        catch (ApplicationException ex) when (ex.Message.StartsWith("stripe_error"))
        {
            logger.LogError(ex, "Stripe error");
            // We capture the controlled code from the helper and return it to the client.
            return new PaymentDto("", "error", dto.AmountCents, dto.Currency)
            {
                ErrorCode = ex.Message.Replace("stripe_error:", "")
            };
        }
        catch (ApplicationException ex) when (ex.Message == "internal_error")
        {
            logger.LogError(ex, "Stripe error");
            return new PaymentDto("", "error", dto.AmountCents, dto.Currency)
            {
                ErrorCode = "internal_error"
            };
        }
    }

    public async Task<PaymentRefundResultDto> RefundPaymentAsync(PaymentRefundDto dto, CancellationToken cancellationToken = default)
    {
        var validator = new CreatePaymentRefundValidator();
        await validator.ValidateAndThrowAsync(dto, cancellationToken);

        var existingRefundId = await cache.GetAsync<int?>(dto.IdempotencyKey);
        if (existingRefundId.HasValue)
        {
            var existing = await context.Set<PaymentRefund>().FindAsync([existingRefundId], cancellationToken);
            if (existing != null)
                return mapper.Map<PaymentRefundResultDto>(existing);
        }

        var payment = await context.Set<Payment>()
            .FirstOrDefaultAsync(p => p.Id == dto.PaymentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Payment with ID {dto.PaymentId} not found.");

        var requestOptions = new RequestOptions { IdempotencyKey = dto.IdempotencyKey };

        try
        {
            var refund = await HandleStripeOperationAsync(
                () => _refundService.CreateAsync(new RefundCreateOptions
                {
                    PaymentIntent = payment.ProviderPaymentId,
                    Amount = dto.AmountCents
                }, requestOptions, cancellationToken),
                "RefundPayment"
            );

            // Register Refund in DB
            var refundEntity = mapper.Map<PaymentRefund>(refund);
            refundEntity.PaymentId = payment.Id;

            await context.Set<PaymentRefund>().AddAsync(refundEntity, cancellationToken);

            if (!string.IsNullOrEmpty(requestOptions.IdempotencyKey))
                await cache.SetAsync(requestOptions.IdempotencyKey, refundEntity.Id, ttl: IdempotencyTtl, slidingExpiration: null);

            // If successful, we update the overall payment status.
            if (refund.Status == "succeeded")
            {
                // Calculate the total refunded so far
                var totalRefunded = await context.Set<PaymentRefund>()
                    .Where(r => r.PaymentId == payment.Id && r.Status == "succeeded")
                    .SumAsync(r => r.AmountCents, cancellationToken);

                totalRefunded += refundEntity.AmountCents;

                if (totalRefunded >= payment.AmountCents)
                    payment.Status = "refunded";
                else if (totalRefunded > 0)
                    payment.Status = "partial_refunded";
            }

            await context.SaveChangesAsync(cancellationToken);

            return new PaymentRefundResultDto(refund.Id!, refund.Status!, refund.Amount, refund.Currency ?? payment.Currency);
        }
        catch (ApplicationException ex) when (ex.Message.StartsWith("stripe_error"))
        {
            logger.LogError(ex, "Error refunding payment {PaymentId}", dto.PaymentId);
            return new PaymentRefundResultDto("", "error", dto.AmountCents, "usd")
            {
                ErrorCode = ex.Message.Replace("stripe_error:", "")
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error refunding payment {PaymentId}", dto.PaymentId);
            return new PaymentRefundResultDto("", "error", dto.AmountCents, "usd")
            {
                ErrorCode = "internal_error"
            };
        }
    }
}
