using Microsoft.AspNetCore.Mvc;

namespace UcarMobileApi.Web.Extensions;

/// <summary>
/// Represents an <see cref="ActionResult"/> that includes additional HTTP headers in the response.
/// </summary>
/// <remarks>
/// This wrapper allows you to append custom headers to an existing result without altering its core behavior.
/// </remarks>
public class ActionResultWithHeaders(ActionResult receiver, IHeaderDictionary headers) : ActionResult
{
    /// <summary>
    /// Gets the collection of HTTP headers to be appended to the response.
    /// </summary>
    public IHeaderDictionary Headers { get; } = headers;
    /// <summary>
    /// Gets the wrapped <see cref="ActionResult"/> that will be executed after headers are appended.
    /// </summary>
    public ActionResult Receiver { get; } = receiver;

    /// <summary>
    /// Appends the specified headers to the HTTP response.
    /// </summary>
    /// <param name="response">The HTTP response to which headers will be added.</param>
    private void AddHeaders(HttpResponse response)
    {
        foreach (var (name, value) in Headers)
            response.Headers.Append(name, value);
    }

    /// <summary>
    /// Executes the result asynchronously and appends the headers to the response.
    /// </summary>
    /// <param name="context">The context in which the result is executed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        AddHeaders(context.HttpContext.Response);
        return Receiver.ExecuteResultAsync(context);
    }

    /// <summary>
    /// Executes the result synchronously and appends the headers to the response.
    /// </summary>
    /// <param name="context">The context in which the result is executed.</param>
    public override void ExecuteResult(ActionContext context)
    {
        AddHeaders(context.HttpContext.Response);
        Receiver.ExecuteResult(context);
    }
}
