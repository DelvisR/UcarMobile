using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure authentication and authorization 
/// using AWS Cognito with JWT Bearer tokens.
/// </summary>
/// <remarks>
/// Registers JWT Bearer authentication against AWS Cognito User Pool 
/// and configures authorization policies with fallback authentication.
/// </remarks>
public static class SecurityConfiguration
{
    /// <summary>
    /// Registers AWS Cognito authentication and authorization policies.
    /// </summary>
    /// <param name="services">The service collection used to register dependencies.</param>
    /// <param name="awsSettings">The application configuration containing AWS Cognito settings.</param>
    /// <returns>The updated service collection.</returns>
    /// <remarks>
    /// This method configures:
    /// <list type="bullet">
    /// <item><description>JWT Bearer authentication with AWS Cognito as authority.</description></item>
    /// <item><description>Validation of issuer, audience, and token lifetime.</description></item>
    /// <item><description>A fallback policy requiring authenticated users for all endpoints.</description></item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddCognitoAuthAndPolicies(this IServiceCollection services, AwsSettings awsSettings)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        var authority = $"https://cognito-idp.{awsSettings.Region}.amazonaws.com/{awsSettings.Cognito.UserPoolId}";
        var clientId = awsSettings.Cognito.ClientId;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = clientId;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = true,
                    ValidAudience = clientId,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(awsSettings.Cognito.ClockSkewMinutes)
                };

                options.Events = new JwtBearerEvents
                {
                    // JWT validation failures (e.g., malformed token)
                    OnAuthenticationFailed = context =>
                    {
                        // Log if you want
                        Log.Warning("JWT validation failed: {Message}", context.Exception?.Message);
                        return Task.CompletedTask;
                    },

                    // Challenge occurs when token is missing or invalid
                    OnChallenge = async context =>
                    {
                        // Check if the endpoint allows anonymous access
                        var endpoint = context.HttpContext.GetEndpoint();
                        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
                        {
                            // Skip challenge, allow anonymous access
                            return;
                        }

                        // Suppress default WWW-Authenticate header
                        context.HandleResponse();

                        // Return consistent 401 JSON response
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            message = "Unauthorized access.",
                            traceId = context.HttpContext.TraceIdentifier
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }

                };
            });

        services.AddAuthorizationBuilder()
            // Require authenticated user for all endpoints by default
            // This is the fallback policy applied to endpoints without an explicit [Authorize] attribute.
            // If you want an endpoint to be publicly accessible (not protected), mark it with [AllowAnonymous].
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }
}