using Serilog;

namespace UcarMobileApi.Configuration
{
    public static class SentryConfiguration
    {
        public static WebApplicationBuilder AddSentryConfiguration(this WebApplicationBuilder builder)
        {
            var dsn = builder.Configuration["Sentry:Dsn"];

            // Case 1: DSN is empty or missing -> Do not enable Sentry
            if (string.IsNullOrWhiteSpace(dsn))
            {
                Log.Warning("⚠️ Sentry DSN is missing. Sentry will not be enabled. Falling back to Serilog only.");
                return builder;
            }

            // Case 2: DSN has an invalid format -> Prevent crash at startup
            if (!Uri.TryCreate(dsn, UriKind.Absolute, out var uri) || !dsn.Contains("@"))
            {
                Log.Warning("⚠️ Sentry DSN format is invalid. Sentry will not be enabled. Falling back to Serilog only.");
                return builder;
            }

            // Case 3: DSN is well-formed (Sentry may still reject it if expired/revoked,
            // but the app will continue running and Serilog will still capture logs)
            builder.WebHost.UseSentry(o =>
            {
                o.Dsn = dsn;
                o.TracesSampleRate = 1.0;

                // Enable extra debug info in development
                o.Debug = builder.Environment.IsDevelopment();

                // Only capture errors and above in diagnostics
                o.DiagnosticLevel = SentryLevel.Error;
            });

            Log.Information("✅ Sentry has been configured successfully.");
            return builder;
        }
    }
}