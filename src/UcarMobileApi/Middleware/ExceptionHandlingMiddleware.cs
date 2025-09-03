using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;
using System.Text.Json;
using UcarMobileApi.Core.Exceptions;

namespace UcarMobileApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
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

                case KeyNotFoundException _:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        message = "Invalid key or argument provided.",
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

                case DbUpdateException dbEx: // For PostgreSQL/EF-related database errors
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = new
                    {
                        message = "Database error occurred.",
                        traceId = context.TraceIdentifier,
                        // Optionally expose dbEx.Message in dev env only; avoid in prod for security
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

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}