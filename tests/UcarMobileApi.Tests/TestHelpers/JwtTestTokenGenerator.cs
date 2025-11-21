using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace UcarMobileApi.Tests.TestHelpers;

/// <summary>
/// Generates mock JWT tokens for integration testing.
/// These tokens mimic the structure of AWS Cognito tokens.
/// </summary>
public static class JwtTestTokenGenerator
{
    private const string Issuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_TestPool";
    private const string Audience = "test-client-id";
    private const string SecretKey = "test-secret-key-for-jwt-token-generation-minimum-256-bits";

    /// <summary>
    /// Generates a mock JWT token for testing purposes.
    /// </summary>
    /// <param name="authProviderId">The Cognito user ID (sub claim).</param>
    /// <param name="email">Optional email claim.</param>
    /// <param name="username">Optional username claim.</param>
    /// <param name="expirationMinutes">Token expiration time in minutes (default 60).</param>
    /// <returns>A JWT token string.</returns>
    public static string GenerateToken(
        string authProviderId,
        string email = null,
        string username = null,
        int expirationMinutes = 60)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, authProviderId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("token_use", "access"),
            new Claim("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        // Add optional claims
        if (!string.IsNullOrEmpty(email))
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));

        if (!string.IsNullOrEmpty(username))
            claims.Add(new Claim("cognito:username", username));

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a token with custom claims.
    /// </summary>
    /// <param name="authProviderId">The Cognito user ID (sub claim).</param>
    /// <param name="customClaims">Additional custom claims.</param>
    /// <param name="expirationMinutes">Token expiration time in minutes.</param>
    /// <returns>A JWT token string.</returns>
    public static string GenerateTokenWithClaims(
        string authProviderId,
        Dictionary<string, string> customClaims,
        int expirationMinutes = 60)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, authProviderId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("token_use", "access"),
            new Claim("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        foreach (var claim in customClaims)
        {
            claims.Add(new Claim(claim.Key, claim.Value));
        }

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates an expired token for testing authorization failures.
    /// </summary>
    /// <param name="authProviderId">The Cognito user ID (sub claim).</param>
    /// <returns>An expired JWT token string.</returns>
    public static string GenerateExpiredToken(string authProviderId)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, authProviderId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("token_use", "access")
        };

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddHours(-2),
            expires: DateTime.UtcNow.AddHours(-1), // Expired 1 hour ago
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
