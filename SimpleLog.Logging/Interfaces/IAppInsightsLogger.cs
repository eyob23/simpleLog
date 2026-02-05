namespace SimpleLog.Logging.Interfaces;

/// <summary>
/// Interface for Application Insights logging operations.
/// This interface abstracts the logging functionality to allow easy testing and decoupling.
/// </summary>
public interface IAppInsightsLogger
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    void LogError(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Logs a critical error message.
    /// </summary>
    void LogCritical(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Tracks a custom event with optional properties and metrics.
    /// </summary>
    void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null);

    /// <summary>
    /// Tracks a custom metric.
    /// </summary>
    void TrackMetric(string metricName, double value, IDictionary<string, string>? properties = null);

    /// <summary>
    /// Tracks a dependency call (e.g., external API, database).
    /// </summary>
    void TrackDependency(string dependencyTypeName, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success);
}
