using Serilog;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides configuration for global exception handling.
/// </summary>
/// <remarks>
/// Sets up centralized exception handlers for logging and returning consistent error responses.
/// </remarks>
public static class GlobalExceptionConfiguration
{
    /// <summary>
    /// Configures global exception handlers outside of the request pipeline.
    /// </summary>
    /// <remarks>
    /// Ensures that unhandled exceptions are logged and transformed into standardized HTTP responses.
    /// </remarks>
    public static void ConfigureGlobalExceptionHandlers()
    {
        // Fatal exceptions at AppDomain level (non-HTTP threads, timers, etc.)
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            Log.Fatal(ex, "Fatal unhandled exception occurred.");

            if (SentrySdk.IsEnabled && ex != null)
            {
                SentrySdk.CaptureException(ex);
            }
        };

        // Unobserved task exceptions
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Log.Error(e.Exception, "Unobserved task exception occurred.");

            if (SentrySdk.IsEnabled)
            {
                SentrySdk.CaptureException(e.Exception);
            }

            e.SetObserved();
        };
    }
}