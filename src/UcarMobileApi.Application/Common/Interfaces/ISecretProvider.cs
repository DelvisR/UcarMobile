using System.Threading.Tasks;

namespace UcarMobileApi.Application.Common.Interfaces;

/// <summary>
/// Interface for retrieving secrets from AWS Secrets Manager
/// </summary>
public interface ISecretProvider
{
    /// <summary>
    /// Retrieves a secret value by its name from AWS Secrets Manager
    /// </summary>
    /// <param name="secretName">The name of the secret to retrieve</param>
    /// <returns>The secret value if found, null otherwise</returns>
    Task<string?> GetSecretAsync(string secretName);
}
