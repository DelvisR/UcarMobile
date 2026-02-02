using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UcarMobileApi.Core.Exceptions;

namespace UcarMobileApi.Middleware
{
    /// <summary>
    /// Middleware that handles unhandled exceptions in the HTTP request pipeline.
    /// </summary>
    /// <remarks>
    /// Captures exceptions, logs them, and returns standardized error responses to clients.
    /// </remarks>
    public class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        /// <summary>
        /// Processes the HTTP request and catches any unhandled exceptions.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Always log with Serilog
                Log.Error(ex, "Unhandled exception. Path: {Path}, Method: {Method}, TraceId: {TraceId}",
                    context.Request.Path, context.Request.Method, context.TraceIdentifier);

                // Report to Sentry if active
                if (SentrySdk.IsEnabled)
                {
                    SentrySdk.CaptureException(ex);
                }

                // Handle the exception and prepare response
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            object response;
            int statusCode;

            switch (ex)
            {
                case ValidationException valEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        message = "Validation failed.",
                        traceId = context.TraceIdentifier,
                        errors = valEx.Errors.Select(e => new
                        {
                            property = e.PropertyName,
                            error = e.ErrorMessage,
                            attemptedValue = e.AttemptedValue
                        })
                    };
                    break;

                case UnauthorizedAccessException _:
                case InvalidOperationException invEx when invEx.Message.Contains("authorization", StringComparison.OrdinalIgnoreCase):
                    // Handle authorization-related exceptions (e.g., custom auth failures)
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        message = "Unauthorized access.",
                        traceId = context.TraceIdentifier
                    };
                    break;

                case ArgumentException argEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        message = argEx.Message,
                        traceId = context.TraceIdentifier
                    };
                    break;

                case KeyNotFoundException argEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        message = argEx.Message,
                        traceId = context.TraceIdentifier
                    };
                    break;

                case NotFoundException notFoundEx:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        message = notFoundEx?.Message ?? "Resource not found.",
                        traceId = context.TraceIdentifier
                    };
                    break;

                case BusinessException businessExceptionEx:
                    statusCode = (int)HttpStatusCode.UnprocessableContent;
                    response = new
                    {
                        message = businessExceptionEx.Message,
                        traceId = context.TraceIdentifier
                    };
                    break;

                case DbUpdateException dbEx: // For PostgreSQL/EF-related database errors
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = new
                    {
                        message = "Database error occurred.",
                        traceId = context.TraceIdentifier,
                        // Optionally expose dbEx.Message in dev env only; avoid in prod for security
                    };
                    break;

                case HttpRequestException httpEx:
                    statusCode = (int)HttpStatusCode.BadGateway;
                    response = new
                    {
                        message = httpEx.Message,
                        traceId = context.TraceIdentifier
                    };
                    break;

                default:
                    // Catch-all for unexpected errors
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = new
                    {
                        message = "An unexpected error occurred.",
                        traceId = context.TraceIdentifier
                    };
                    break;
            }

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    /// <summary>
    /// Provides extension methods to configure the <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <remarks>
    /// Enables centralized exception handling in the request pipeline.
    /// </remarks>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="ExceptionHandlingMiddleware"/> to the application's request pipeline.
        /// </summary>
        /// <param name="app">The application builder used to configure middleware.</param>
        /// <returns>The updated application builder.</returns>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
