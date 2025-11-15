using Microsoft.AspNetCore.Mvc;

namespace UcarMobileApi.Web.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ActionResult"/> to enhance response behavior.
/// </summary>
public static class ActionResultExtensions
{
    /// <summary>
    /// Wraps the specified <see cref="ActionResult"/> with additional HTTP headers.
    /// </summary>
    /// <param name="receiver">The original <see cref="ActionResult"/> to be extended.</param>
    /// <param name="headers">The collection of HTTP headers to include in the response.</param>
    /// <returns>
    /// A new <see cref="ActionResult"/> instance that includes the specified headers.
    /// </returns>
    /// <remarks>
    /// This method is useful for appending custom headers to an existing result without modifying its core behavior.
    /// </remarks>
    public static ActionResult WithHeaders(this ActionResult receiver, IHeaderDictionary headers)
    {
        return new ActionResultWithHeaders(receiver, headers);
    }
}
