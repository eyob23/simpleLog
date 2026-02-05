using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleLog.Logging.Implementation;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Logging.Extensions;

/// <summary>
/// Extension methods for configuring Azure Monitor OpenTelemetry logging services.
/// </summary>
public static class AzureMonitorServiceExtensions
{
    /// <summary>
    /// Adds Azure Monitor OpenTelemetry logging services with configuration lookup.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAzureMonitorOpenTelemetryLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["AzureMonitor:ConnectionString"]
            ?? configuration["ApplicationInsights:ConnectionString"];

        return services.AddAzureMonitorOpenTelemetryLogging(connectionString);
    }

    /// <summary>
    /// Adds Azure Monitor OpenTelemetry logging services with optional connection string.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The Azure Monitor connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAzureMonitorOpenTelemetryLogging(this IServiceCollection services, string? connectionString = null)
    {
        var builder = services.AddOpenTelemetry();

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            builder.UseAzureMonitor(options =>
            {
                options.ConnectionString = connectionString;
            });
        }
        else
        {
            builder.UseAzureMonitor();
        }

        services.AddSingleton<IAzureMonitorLogger, AzureMonitorLogger>();

        return services;
    }
}
