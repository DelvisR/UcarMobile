using Serilog;

namespace UcarMobileApi.Configuration
{
    /// <summary>
    /// Provides extension methods to configure Serilog logging.
    /// </summary>
    /// <remarks>
    /// Configures Serilog from application settings and integrates it with ASP.NET Core logging.
    /// </remarks>
    public static class SerilogConfiguration
    {
        /// <summary>
        /// Adds and configures Serilog as the logging provider.
        /// </summary>
        /// <param name="builder">The web application builder used to configure logging.</param>
        /// <returns>The updated web application builder.</returns>
        public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
        {
            try
            {
                // Load Serilog configuration from appsettings.json
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();

                builder.Host.UseSerilog();

                Log.Information("Serilog initialized from appsettings.json.");
            }
            catch (Exception ex)
            {
                // If Serilog fails to load from config, fallback to console only
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .Enrich.FromLogContext()
                    .CreateLogger();

                builder.Host.UseSerilog();

                Log.Warning(ex, "Failed to load Serilog from configuration. Falling back to console logging.");
            }
        }
    }
}
