using Serilog;

namespace UcarMobileApi.Configuration
{
    public static class SerilogConfiguration
    {
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

                Log.Information("✅ Serilog initialized from appsettings.json.");
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

                Log.Warning(ex, "⚠️ Failed to load Serilog from configuration. Falling back to console logging.");
            }
        }
    }
}