using Serilog;

namespace UcarMobileApi.Configuration
{
    /// <summary>
    /// Provides extension methods to configure Sentry for error monitoring and tracing.
    /// </summary>
    /// <remarks>
    /// Adds and configures Sentry integration for capturing exceptions and performance metrics.
    /// </remarks>
    public static class SentryConfiguration
    {
        /// <summary>
        /// Adds Sentry error monitoring services to the application.
        /// </summary>
        /// <param name="builder">The web application builder used to configure services and middleware.</param>
        /// <returns>The updated web application builder.</returns>
        public static WebApplicationBuilder AddSentryConfiguration(this WebApplicationBuilder builder)
        {

            var sentryConfig = builder.Configuration.GetSection("Sentry");

            var dsn = sentryConfig["Dsn"];

            // Case 1: DSN is empty or missing -> Do not enable Sentry
            if (string.IsNullOrWhiteSpace(dsn))
            {
                Log.Warning("Sentry DSN is missing. Sentry will not be enabled. Falling back to Serilog only.");
                return builder;
            }

            // Case 2: DSN has an invalid format -> Prevent crash at startup
            if (!Uri.TryCreate(dsn, UriKind.Absolute, out var uri) || !dsn.Contains('@'))
            {
                Log.Warning("Sentry DSN format is invalid. Sentry will not be enabled. Falling back to Serilog only.");
                return builder;
            }

            // Case 3: DSN is well-formed (Sentry may still reject it if expired/revoked,
            // but the app will continue running and Serilog will still capture logs)
            builder.WebHost.UseSentry(o =>
            {
                o.Dsn = dsn;
                o.SendDefaultPii = bool.Parse(sentryConfig["SendDefaultPii"] ?? "false");
                o.MinimumBreadcrumbLevel = Enum.Parse<LogLevel>(sentryConfig["MinimumBreadcrumbLevel"] ?? "Info");
                o.MinimumEventLevel = Enum.Parse<LogLevel>(sentryConfig["MinimumEventLevel"] ?? "Error");
                o.AttachStacktrace = bool.Parse(sentryConfig["AttachStackTrace"] ?? "false");
                o.Debug = builder.Environment.IsDevelopment();
                o.DiagnosticLevel = Enum.Parse<SentryLevel>(sentryConfig["DiagnosticsLevel"] ?? "Error");
                o.Environment = builder.Environment.EnvironmentName;
            });

            Log.Information("Sentry has been configured successfully.");
            return builder;
        }
    }
}
