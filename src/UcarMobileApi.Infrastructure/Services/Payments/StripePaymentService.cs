using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Payments;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Application.Validators;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Core.Entities.Technicians;
using UcarMobileApi.Core.Exceptions;
using UcarMobileApi.Infrastructure.Data;
using PaymentMethod = UcarMobileApi.Core.Entities.Payments.PaymentMethod;
using Payout = UcarMobileApi.Core.Entities.Payments.Payout;

namespace UcarMobileApi.Infrastructure.Services.Payments;

/// <summary>
/// Stripe-based implementation of <see cref="IPaymentService"/>.
/// Manages setup intents, payment methods, charges, and refunds using Stripe API.
/// </summary>
public class StripePaymentService(
    AppDbContext context,
    IMapper mapper,
    ILogger<StripePaymentService> logger,
    ICacheService cache,
    CustomerService? customerService = null,
    SetupIntentService? setupIntentService = null,
    PaymentMethodService? paymentMethodService = null,
    PaymentIntentService? paymentIntentService = null,
    RefundService? refundService = null)
    : IPaymentService
{
    private readonly CustomerService _customerService = customerService ?? new CustomerService();
    private readonly SetupIntentService _setupIntentService = setupIntentService ?? new SetupIntentService();
    private readonly PaymentMethodService _paymentMethodService = paymentMethodService ?? new PaymentMethodService();
    private readonly PaymentIntentService _paymentIntentService = paymentIntentService ?? new PaymentIntentService();
    private readonly RefundService _refundService = refundService ?? new RefundService();

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

    public async Task<IEnumerable<PaymentMethodListDto>> GetPaymentMethodsAsync(string authProviderId, CancellationToken cancellationToken = default)
    {
        var client = await context.Set<Client>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken);

        if (client == null) return [];

        return await context.Set<PaymentMethod>()
            .Where(pm => pm.ClientId == client.Id && !pm.IsDeleted)
            .AsNoTracking()
            .OrderByDescending(pm => pm.IsDefault)   // default first
            .ThenBy(pm => pm.Brand)
            .ProjectTo<PaymentMethodListDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

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

    public async Task SetDefaultPaymentMethodAsync(string authProviderId, int paymentMethodId, CancellationToken cancellationToken = default)
    {
        var client = await context.Set<Client>()
                         .AsNoTracking()
                         .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Client with AuthProviderId {authProviderId} not found.");

        var belongsToClient = await context.Set<PaymentMethod>()
            .AnyAsync(pm => pm.Id == paymentMethodId && pm.ClientId == client.Id, cancellationToken);

        if (!belongsToClient)
            throw new KeyNotFoundException($"PaymentMethod with Id {paymentMethodId} not found for the current user.");

        // Obtener todos los métodos del cliente
        var allMethods = await context.Set<PaymentMethod>()
            .Where(pm => pm.ClientId == client.Id)
            .ToListAsync(cancellationToken);

        foreach (var m in allMethods)
            m.IsDefault = m.Id == paymentMethodId;

        await context.SaveChangesAsync(cancellationToken);
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

        var client = await context.Set<Client>()
                         .AsNoTracking()
                         .Include(c => c.PaymentMethods)
                         .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Client with AuthProviderId {authProviderId} not found.");

        if (string.IsNullOrEmpty(client.ProviderPaymentCustomerId))
            throw new InvalidOperationException("Client does not have an associated Stripe customer ID.");

        var paymentMethod = client.PaymentMethods.FirstOrDefault(pm => pm.Id == dto.PaymentMethodId)
                            ?? throw new KeyNotFoundException("Payment method not found for this client.");

        // Generate RequestOptions with idempotency
        var requestOptions = new RequestOptions { IdempotencyKey = dto.IdempotencyKey };

        var options = new PaymentIntentCreateOptions
        {
            Customer = client.ProviderPaymentCustomerId,
            PaymentMethod = paymentMethod.ProviderPaymentMethodId,
            Amount = dto.AmountCents,
            Currency = dto.Currency ?? "usd",
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
            return new PaymentDto("", "error", dto.AmountCents, dto.Currency ?? "usd")
            {
                ErrorCode = ex.Message.Replace("stripe_error:", "")
            };
        }
        catch (ApplicationException ex) when (ex.Message == "internal_error")
        {
            logger.LogError(ex, "Stripe error");
            return new PaymentDto("", "error", dto.AmountCents, dto.Currency ?? "usd")
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

    #region // ======== PAYOUTS ========== //

    public async Task<OnboardResponseDto> CreateAccountAsync(string authProviderId, OnboardRequestDto dto, CancellationToken cancellationToken)
    {
        var technician = await context.Set<Technician>()
                             .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Technician with AuthProviderId {authProviderId} not found.");

        if (!string.IsNullOrEmpty(technician.ProviderAccountId))
            throw new InvalidOperationException("Technician already has a provider account.");

        var accountService = new AccountService();
        var account = await accountService.CreateAsync(new AccountCreateOptions
        {
            Type = "express",
            Email = technician.Email,
            BusinessType = "individual"
        }, cancellationToken: cancellationToken);

        technician.ProviderAccountId = account.Id;
        await context.SaveChangesAsync(cancellationToken);

        var link = await CreateAccountLinkInternalAsync(account.Id, dto, cancellationToken);

        return new OnboardResponseDto { OnboardingUrl = link };
    }

    /// <summary>
    /// Generates a new onboarding link for an existing provider account.
    /// </summary>
    public async Task<OnboardResponseDto> RefreshOnboardingLinkAsync(string authProviderId, OnboardRequestDto dto, CancellationToken cancellationToken)
    {
        var technician = await context.Set<Technician>().AsNoTracking()
                             .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Technician with AuthProviderId {authProviderId} not found.");

        if (string.IsNullOrEmpty(technician.ProviderAccountId))
            throw new InvalidOperationException("Technician has no provider account.");

        var link = await CreateAccountLinkInternalAsync(technician.ProviderAccountId, dto, cancellationToken);

        return new OnboardResponseDto { OnboardingUrl = link };
    }

    /// <summary>
    /// Creates an account onboarding link using the provider API.
    /// </summary>
    private static async Task<string> CreateAccountLinkInternalAsync(string providerAccountId, OnboardRequestDto dto, CancellationToken cancellationToken)
    {
        var linkService = new AccountLinkService();
        var link = await linkService.CreateAsync(new AccountLinkCreateOptions
        {
            Account = providerAccountId,
            RefreshUrl = dto.RefreshUrl,
            ReturnUrl = dto.ReturnUrl,
            Type = "account_onboarding"
        }, cancellationToken: cancellationToken);

        return link.Url;
    }

    /// <summary>
    /// Creates a payout transfer to a technician's connected account in Stripe.
    /// Handles Stripe errors in a controlled manner and persists the payout locally.
    /// </summary>
    /// <param name="authProviderId">The technician provider id</param>
    /// <param name="dto">Data needed for the payout.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="PayoutDto"/> with status and error information.</returns>
    public async Task<PayoutDto> MakeTransferAsync(string authProviderId, PayoutCreateDto dto, CancellationToken cancellationToken)
    {
        var technician = await context.Set<Technician>().AsNoTracking()
                             .FirstOrDefaultAsync(c => c.AuthProviderId == authProviderId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Technician with AuthProviderId {authProviderId} not found.");

        if (string.IsNullOrEmpty(technician.ProviderAccountId))
            throw new BusinessException("Technician has no provider account.");

        if (!technician.ProviderPaymentsEnabled)
            throw new BusinessException("Technician payouts are not enabled.");

        // Verify local idempotence
        var existingPayoutId = await cache.GetAsync<int?>(dto.IdempotencyKey);
        if (existingPayoutId.HasValue)
        {
            var existing = await context.Set<Payout>().FindAsync([existingPayoutId], cancellationToken);
            if (existing != null)
                return mapper.Map<PayoutDto>(existing);
        }

        var transferService = new TransferService();
        var requestOptions = new RequestOptions
        {
            IdempotencyKey = dto.IdempotencyKey
        };

        Transfer? transferResult;

        try
        {
            transferResult = await transferService.CreateAsync(
                new TransferCreateOptions
                {
                    Amount = dto.AmountCents,
                    Currency = dto.Currency ?? "usd",
                    Destination = technician.ProviderAccountId,
                    TransferGroup = $"tech_payout_{technician.ProviderAccountId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "technicianId", technician.Id.ToString() },
                        { "source", "UcarMobile" },
                        { "idempotencyKey", dto.IdempotencyKey }
                    }
                },
                requestOptions,
                cancellationToken
            );
            ;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error while creating transfer");

            var errorCode = ex.StripeError?.Code ?? "stripe_error";

            return new PayoutDto
            {
                Status = "error",
                AmountCents = dto.AmountCents,
                Currency = dto.Currency ?? "usd",
                ErrorCode = errorCode
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected internal error while creating transfer");

            return new PayoutDto
            {
                Status = "error",
                AmountCents = dto.AmountCents,
                Currency = dto.Currency ?? "usd",
                ErrorCode = "internal_error"
            };
        }

        // Persist the payout locally
        var payout = new Payout
        {
            TechnicianId = technician.Id,
            ProviderPayoutId = transferResult.Id,
            AmountCents = dto.AmountCents,
            Currency = dto.Currency ?? "usd",
            Status = "pending"
        };

        await context.Set<Payout>().AddAsync(payout, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Save to local cache
        if (!string.IsNullOrEmpty(requestOptions.IdempotencyKey))
            await cache.SetAsync(requestOptions.IdempotencyKey, payout.Id, ttl: IdempotencyTtl, slidingExpiration: null);

        return mapper.Map<PayoutDto>(payout);
    }

    #endregion

}
