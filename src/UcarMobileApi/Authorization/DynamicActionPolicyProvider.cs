using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace UcarMobileApi.Authorization;

/// <summary>
/// Provides authorization policies dynamically based on action requirements.
/// </summary>
/// <remarks>
/// This policy provider generates authorization policies at runtime when a policy name 
/// starts with the prefix "Action:". It eliminates the need to register all action policies manually.
/// </remarks>
public class DynamicActionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _options = options.Value;

    /// <summary>
    /// Retrieves an authorization policy for the given policy name.
    /// </summary>
    /// <param name="policyName">The name of the policy to retrieve.</param>
    /// <returns>The corresponding <see cref="AuthorizationPolicy"/> if found or dynamically created, otherwise null.</returns>
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (_options.GetPolicy(policyName) is { } existingPolicy)
        {
            return Task.FromResult<AuthorizationPolicy?>(existingPolicy);
        }

        if (!policyName.StartsWith("Action:", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult<AuthorizationPolicy?>(null);

        var action = policyName["Action:".Length..];

        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new ActionRequirement(action))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);

    }
}
