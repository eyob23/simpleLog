using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Logging.Implementation;

/// <summary>
/// Service for logging custom events to Azure Monitor with OpenTelemetry.
/// This service can be injected into controllers and handles the custom event pattern.
/// </summary>
public class CustomEventLogger : ICustomEventLogger
{
    private readonly ILogger<CustomEventLogger> _logger;

    public CustomEventLogger(ILogger<CustomEventLogger> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs a custom event with the specified event name and optional attributes.
    /// </summary>
    /// <param name="eventName">The name of the custom event.</param>
    /// <param name="attributes">Optional additional attributes to include in the event.</param>
    /// <example>
    /// var customEventLogger = new CustomEventLogger(logger);
    /// customEventLogger.LogCustomEvent("user-login", "admin", "192.168.1.1");
    /// </example>
    public void LogCustomEvent(string eventName, params object[] attributes)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));
        }

        var activity = Activity.Current;
        if (activity != null)
        {
            // Build tags from attributes
            var tags = new ActivityTagsCollection();
            if (attributes != null && attributes.Length > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    tags[$"attr{i}"] = attributes[i];
                }
            }

            // Add event to activity - this creates a custom event in Application Insights
            activity.AddEvent(new ActivityEvent(eventName, tags: tags));
        }

        // Also log for consistency
        _logger.LogInformation("Custom event: {eventName}", eventName);
    }

    /// <summary>
    /// Logs a custom event with the specified event name, log level, and optional attributes.
    /// </summary>
    /// <param name="eventName">The name of the custom event.</param>
    /// <param name="logLevel">The log level for the event.</param>
    /// <param name="attributes">Optional additional attributes to include in the event.</param>
    public void LogCustomEvent(string eventName, LogLevel logLevel, params object[] attributes)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));
        }

        var activity = Activity.Current;
        if (activity != null)
        {
            // Build tags from attributes
            var tags = new ActivityTagsCollection();
            if (attributes != null && attributes.Length > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    tags[$"attr{i}"] = attributes[i];
                }
            }

            // Add event to activity - this creates a custom event in Application Insights
            activity.AddEvent(new ActivityEvent(eventName, tags: tags));
        }

        // Also log with specified level for consistency
        _logger.Log(logLevel, "Custom event: {eventName}", eventName);
    }

    /// <summary>
    /// Logs a custom event with the specified event name, exception, and optional attributes.
    /// </summary>
    /// <param name="eventName">The name of the custom event.</param>
    /// <param name="exception">The exception associated with the event.</param>
    /// <param name="attributes">Optional additional attributes to include in the event.</param>
    public void LogCustomEvent(string eventName, Exception exception, params object[] attributes)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));
        }

        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var activity = Activity.Current;
        if (activity != null)
        {
            // Build tags from attributes, including exception info
            var tags = new ActivityTagsCollection();
            tags["exception.type"] = exception.GetType().FullName ?? "Unknown";
            tags["exception.message"] = exception.Message;

            if (attributes != null && attributes.Length > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    tags[$"attr{i}"] = attributes[i];
                }
            }

            // Add event to activity - this creates a custom event in Application Insights
            activity.AddEvent(new ActivityEvent(eventName, tags: tags));
        }

        // Also log error for consistency
        _logger.LogError(exception, "Custom event: {eventName}", eventName);
    }
}


