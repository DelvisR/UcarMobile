using Serilog;

namespace UcarMobileApi.Configuration;

public static class GlobalExceptionConfiguration
{
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