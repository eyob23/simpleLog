using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using SimpleLog.Logging.Implementation;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Logging.Extensions;

/// <summary>
/// Extension methods for configuring SimpleLog logging services.
/// This class provides convenient methods to register logging services in the DI container.
/// </summary>
public static class LoggingServiceExtensions
{
    /// <summary>
    /// Adds SimpleLog Application Insights logging services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSimpleLogLogging(this IServiceCollection services)
    {
        // Register the custom logger as a singleton
        services.AddSingleton<IAppInsightsLogger, AppInsightsLogger>();

        return services;
    }

    /// <summary>
    /// Adds SimpleLog Application Insights logging services with Application Insights configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The Application Insights connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSimpleLogLogging(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Application Insights connection string cannot be null or empty.", nameof(connectionString));
        }

        // Add Application Insights telemetry
        services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = connectionString;
        });

        // Register the custom logger
        services.AddSingleton<IAppInsightsLogger, AppInsightsLogger>();

        return services;
    }
}
