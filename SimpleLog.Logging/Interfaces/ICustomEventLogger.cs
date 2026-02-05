using Microsoft.Extensions.Logging;

namespace SimpleLog.Logging.Interfaces;

/// <summary>
/// Interface for logging custom events to Azure Monitor with OpenTelemetry.
/// </summary>
public interface ICustomEventLogger
{
    /// <summary>
    /// Logs a custom event with the specified event name and optional attributes.
    /// </summary>
    /// <param name="eventName">The name of the custom event.</param>
    /// <param name="attributes">Optional additional attributes to include in the event.</param>
    void LogCustomEvent(string eventName, params object[] attributes);

    /// <summary>
    /// Logs a custom event with the specified event name, log level, and optional attributes.
    /// </summary>
    /// <param name="eventName">The name of the custom event.</param>
    /// <param name="logLevel">The log level for the event.</param>
    /// <param name="attributes">Optional additional attributes to include in the event.</param>
    void LogCustomEvent(string eventName, LogLevel logLevel, params object[] attributes);

    /// <summary>
    /// Logs a custom event with the specified event name, exception, and optional attributes.
    /// </summary>
    /// <param name="eventName">The name of the custom event.</param>
    /// <param name="exception">The exception associated with the event.</param>
    /// <param name="attributes">Optional additional attributes to include in the event.</param>
    void LogCustomEvent(string eventName, Exception exception, params object[] attributes);
}
