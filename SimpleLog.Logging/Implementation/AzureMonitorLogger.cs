using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Logging.Implementation;

/// <summary>
/// Implementation of Azure Monitor OpenTelemetry logger.
/// This class wraps ILogger and emits OpenTelemetry activities and metrics.
/// </summary>
public class AzureMonitorLogger : IAzureMonitorLogger
{
    private static readonly ActivitySource ActivitySource = new("SimpleLog.AzureMonitor");
    private static readonly Meter Meter = new("SimpleLog.AzureMonitor");
    private static readonly ConcurrentDictionary<string, Histogram<double>> Histograms = new();

    private readonly ILogger<AzureMonitorLogger> _logger;

    public AzureMonitorLogger(ILogger<AzureMonitorLogger> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
        AddLogEvent("Information", message, null);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
        AddLogEvent("Warning", message, null);
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
        {
            _logger.LogError(exception, message, args);
        }
        else
        {
            _logger.LogError(message, args);
        }

        AddLogEvent("Error", message, exception);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
        AddLogEvent("Debug", message, null);
    }

    public void LogCritical(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
        {
            _logger.LogCritical(exception, message, args);
        }
        else
        {
            _logger.LogCritical(message, args);
        }

        AddLogEvent("Critical", message, exception);
    }

    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        using var activity = ActivitySource.StartActivity(eventName, ActivityKind.Internal);

        if (activity == null)
        {
            return;
        }

        if (properties != null)
        {
            foreach (var property in properties)
            {
                activity.SetTag(property.Key, property.Value);
            }
        }

        if (metrics != null)
        {
            foreach (var metric in metrics)
            {
                activity.SetTag($"metric.{metric.Key}", metric.Value);
            }
        }

        activity.AddEvent(new ActivityEvent(eventName));
    }

    public void TrackMetric(string metricName, double value, IDictionary<string, string>? properties = null)
    {
        var histogram = Histograms.GetOrAdd(metricName, name => Meter.CreateHistogram<double>(name));
        histogram.Record(value, ToTags(properties));
    }

    public void TrackDependency(string dependencyTypeName, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success)
    {
        var tags = new ActivityTagsCollection
        {
            { "dependency.type", dependencyTypeName },
            { "dependency.name", dependencyName },
            { "dependency.data", data },
            { "dependency.success", success }
        };

        var activity = ActivitySource.StartActivity(
            dependencyName,
            ActivityKind.Client,
            parentContext: default,
            tags: tags,
            links: null,
            startTime: startTime.UtcDateTime);

        if (activity == null)
        {
            return;
        }

        if (!success)
        {
            activity.SetStatus(ActivityStatusCode.Error);
        }

        activity.SetEndTime((startTime + duration).UtcDateTime);
        activity.Stop();
    }

    private static TagList ToTags(IDictionary<string, string>? properties)
    {
        if (properties == null || properties.Count == 0)
        {
            return default;
        }

        var tags = new TagList();
        foreach (var property in properties)
        {
            tags.Add(property.Key, property.Value);
        }

        return tags;
    }

    private static void AddLogEvent(string level, string message, Exception? exception)
    {
        var activity = Activity.Current;
        if (activity == null)
        {
            return;
        }

        var tags = new ActivityTagsCollection
        {
            { "log.level", level },
            { "log.message", message }
        };

        if (exception != null)
        {
            tags.Add("exception.type", exception.GetType().FullName);
            tags.Add("exception.message", exception.Message);
        }

        activity.AddEvent(new ActivityEvent("log", tags: tags));
    }
}
