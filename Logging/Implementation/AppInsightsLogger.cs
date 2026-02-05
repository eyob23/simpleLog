using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Logging.Implementation;

/// <summary>
/// Implementation of Application Insights logger.
/// This class wraps Application Insights TelemetryClient and ILogger for comprehensive logging.
/// </summary>
public class AppInsightsLogger : IAppInsightsLogger
{
    private readonly ILogger<AppInsightsLogger> _logger;
    private readonly TelemetryClient _telemetryClient;

    public AppInsightsLogger(ILogger<AppInsightsLogger> logger, TelemetryClient telemetryClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
        _telemetryClient.TrackTrace(message, SeverityLevel.Information);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
        _telemetryClient.TrackTrace(message, SeverityLevel.Warning);
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
        {
            _logger.LogError(exception, message, args);
            _telemetryClient.TrackException(exception, new Dictionary<string, string>
            {
                { "Message", message }
            });
        }
        else
        {
            _logger.LogError(message, args);
            _telemetryClient.TrackTrace(message, SeverityLevel.Error);
        }
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
        _telemetryClient.TrackTrace(message, SeverityLevel.Verbose);
    }

    public void LogCritical(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
        {
            _logger.LogCritical(exception, message, args);
            _telemetryClient.TrackException(exception, new Dictionary<string, string>
            {
                { "Message", message },
                { "Severity", "Critical" }
            });
        }
        else
        {
            _logger.LogCritical(message, args);
            _telemetryClient.TrackTrace(message, SeverityLevel.Critical);
        }
    }

    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        _telemetryClient.TrackEvent(eventName, properties, metrics);
    }

    public void TrackMetric(string metricName, double value, IDictionary<string, string>? properties = null)
    {
        _telemetryClient.TrackMetric(metricName, value, properties);
    }

    public void TrackDependency(string dependencyTypeName, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success)
    {
        _telemetryClient.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success);
    }
}
