using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace SimpleLog.Logging.Extensions;

/// <summary>
/// Extension methods for configuring Serilog logging services.
/// </summary>
public static class SerilogServiceExtensions
{
    /// <summary>
    /// Configures Serilog as the logging provider with Application Insights integration.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The host builder for chaining.</returns>
    public static IHostBuilder AddSerilogLogging(this IHostBuilder builder, IConfiguration configuration)
    {
        return builder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var serviceName = context.Configuration["Serilog:Properties:Application"] ?? "SimpleLog.Api";

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", serviceName)
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/simplelog-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    shared: true);
        });
    }

    /// <summary>
    /// Creates the initial bootstrap logger for application startup.
    /// </summary>
    /// <param name="serviceName">Optional service name. Defaults to "SimpleLog.Api" if not provided.</param>
    /// <returns>The bootstrap logger configuration.</returns>
    public static Serilog.ILogger CreateBootstrapLogger(string? serviceName = null)
    {
        serviceName ??= "SimpleLog.Api";

        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", serviceName)
            .WriteTo.Console()
            .WriteTo.File(
                path: "Logs/simplelog-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true)
            .CreateBootstrapLogger();
    }
}
