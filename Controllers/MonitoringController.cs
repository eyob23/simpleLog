using Microsoft.AspNetCore.Mvc;
using SimpleLog.Logging.Interfaces;
using System.Diagnostics;

namespace SimpleLog.Api.Controllers;

/// <summary>
/// Controller demonstrating Azure Monitor OpenTelemetry logging capabilities.
/// </summary>
[ApiController]
[Route("api/monitoring")]
public class MonitoringController : ControllerBase
{
    private readonly IAzureMonitorLogger _azureMonitorLogger;

    public MonitoringController(IAzureMonitorLogger azureMonitorLogger)
    {
        _azureMonitorLogger = azureMonitorLogger ?? throw new ArgumentNullException(nameof(azureMonitorLogger));
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    [HttpPost("log-info")]
    public IActionResult LogInformation([FromBody] LogMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
        {
            return BadRequest(new { error = "Message is required" });
        }

        _azureMonitorLogger.LogInformation(request.Message);
        return Ok(new { message = "Information logged successfully" });
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    [HttpPost("log-warning")]
    public IActionResult LogWarning([FromBody] LogMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
        {
            return BadRequest(new { error = "Message is required" });
        }

        _azureMonitorLogger.LogWarning(request.Message);
        return Ok(new { message = "Warning logged successfully" });
    }

    /// <summary>
    /// Logs an error message with optional exception details.
    /// </summary>
    [HttpPost("log-error")]
    public IActionResult LogError([FromBody] LogErrorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
        {
            return BadRequest(new { error = "Message is required" });
        }

        Exception? exception = null;
        if (!string.IsNullOrWhiteSpace(request.ExceptionMessage))
        {
            exception = new Exception(request.ExceptionMessage);
        }

        _azureMonitorLogger.LogError(request.Message, exception);
        return Ok(new { message = "Error logged successfully" });
    }

    /// <summary>
    /// Tracks a custom event with properties and metrics.
    /// </summary>
    [HttpPost("track-event")]
    public IActionResult TrackEvent([FromBody] TrackEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.EventName))
        {
            return BadRequest(new { error = "EventName is required" });
        }

        _azureMonitorLogger.TrackEvent(
            request.EventName,
            request.Properties,
            request.Metrics
        );

        return Ok(new { message = "Event tracked successfully" });
    }

    /// <summary>
    /// Tracks a custom metric.
    /// </summary>
    [HttpPost("track-metric")]
    public IActionResult TrackMetric([FromBody] TrackMetricRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.MetricName))
        {
            return BadRequest(new { error = "MetricName is required" });
        }

        _azureMonitorLogger.TrackMetric(
            request.MetricName,
            request.Value,
            request.Properties
        );

        return Ok(new { message = "Metric tracked successfully" });
    }

    /// <summary>
    /// Tracks a dependency call.
    /// </summary>
    [HttpPost("track-dependency")]
    public IActionResult TrackDependency([FromBody] TrackDependencyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.DependencyTypeName))
        {
            return BadRequest(new { error = "DependencyTypeName is required" });
        }

        if (string.IsNullOrWhiteSpace(request?.DependencyName))
        {
            return BadRequest(new { error = "DependencyName is required" });
        }

        _azureMonitorLogger.TrackDependency(
            request.DependencyTypeName,
            request.DependencyName,
            request.Data ?? string.Empty,
            DateTimeOffset.UtcNow.AddSeconds(-request.DurationSeconds),
            TimeSpan.FromSeconds(request.DurationSeconds),
            request.Success
        );

        return Ok(new { message = "Dependency tracked successfully" });
    }

    /// <summary>
    /// Logs a custom event using OpenTelemetry semantic convention.
    /// </summary>
    [HttpPost("custom-event")]
    public IActionResult LogCustomEvent([FromBody] AzureMonitorEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.EventName))
        {
            return BadRequest(new { error = "EventName is required" });
        }

        if (!string.IsNullOrWhiteSpace(request.AdditionalAttribute))
        {
            _azureMonitorLogger.LogCustomEvent(request.EventName, request.AdditionalAttribute);
        }
        else
        {
            _azureMonitorLogger.LogCustomEvent(request.EventName);
        }

        return Ok(new { message = "Custom event logged successfully" });
    }

    /// <summary>
    /// Simulates an operation with multiple monitoring points.
    /// </summary>
    [HttpPost("simulate-operation")]
    public IActionResult SimulateOperation([FromBody] SimulateOperationRequest request)
    {
        var operationName = request?.OperationName ?? "TestOperation";
        var startTime = DateTimeOffset.UtcNow;

        _azureMonitorLogger.LogInformation($"Starting operation: {operationName}");

        try
        {
            // Simulate some work
            Thread.Sleep(100);

            // Track a custom event
            _azureMonitorLogger.TrackEvent(
                $"{operationName}Started",
                new Dictionary<string, string> { { "Operation", operationName } },
                new Dictionary<string, double> { { "DurationMs", 100 } }
            );

            // Simulate a dependency call
            var dependencyStartTime = DateTimeOffset.UtcNow;
            Thread.Sleep(50);
            _azureMonitorLogger.TrackDependency(
                "HTTP",
                "ExternalAPI",
                "GET /api/data",
                dependencyStartTime,
                TimeSpan.FromMilliseconds(50),
                true
            );

            // Track a metric
            _azureMonitorLogger.TrackMetric(
                $"{operationName}.ExecutionTime",
                150,
                new Dictionary<string, string> { { "Status", "Success" } }
            );

            _azureMonitorLogger.LogInformation($"Operation completed successfully: {operationName}");

            return Ok(new
            {
                message = "Operation simulated and monitored successfully",
                operationName,
                duration = DateTimeOffset.UtcNow - startTime
            });
        }
        catch (Exception ex)
        {
            _azureMonitorLogger.LogError($"Operation failed: {operationName}", ex);
            throw;
        }
    }
}

// Request DTOs
public class LogMessageRequest
{
    public string Message { get; set; } = string.Empty;
}

public class LogErrorRequest
{
    public string Message { get; set; } = string.Empty;
    public string? ExceptionMessage { get; set; }
}

public class TrackEventRequest
{
    public string EventName { get; set; } = string.Empty;
    public Dictionary<string, string>? Properties { get; set; }
    public Dictionary<string, double>? Metrics { get; set; }
}

public class TrackMetricRequest
{
    public string MetricName { get; set; } = string.Empty;
    public double Value { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
}

public class TrackDependencyRequest
{
    public string DependencyTypeName { get; set; } = string.Empty;
    public string DependencyName { get; set; } = string.Empty;
    public string? Data { get; set; }
    public double DurationSeconds { get; set; } = 1.0;
    public bool Success { get; set; } = true;
}

public class AzureMonitorEventRequest
{
    public string EventName { get; set; } = string.Empty;
    public string? AdditionalAttribute { get; set; }
}

public class SimulateOperationRequest
{
    public string? OperationName { get; set; }
}
