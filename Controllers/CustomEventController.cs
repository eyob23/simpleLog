using Microsoft.AspNetCore.Mvc;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Api.Controllers;

/// <summary>
/// Example controller demonstrating how to use the ICustomEventLogger service
/// for logging custom events to Azure Monitor.
/// </summary>
[ApiController]
[Route("api/custom-events")]
public class CustomEventController : ControllerBase
{
    private readonly ICustomEventLogger _customEventLogger;

    public CustomEventController(ICustomEventLogger customEventLogger)
    {
        _customEventLogger = customEventLogger ?? throw new ArgumentNullException(nameof(customEventLogger));
    }

    /// <summary>
    /// Logs a simple custom event without additional attributes.
    /// </summary>
    /// <example>
    /// POST /api/custom-events/simple
    /// Response: 200 OK
    /// </example>
    [HttpPost("simple")]
    public IActionResult LogSimpleEvent()
    {
        _customEventLogger.LogCustomEvent("user-action");
        return Ok(new { message = "Custom event logged successfully" });
    }

    /// <summary>
    /// Logs a custom event with additional attributes.
    /// </summary>
    /// <param name="request">The event details</param>
    /// <example>
    /// POST /api/custom-events/with-attributes
    /// Body: { "eventName": "user-login", "userId": "user123", "ipAddress": "192.168.1.1" }
    /// Response: 200 OK
    /// </example>
    [HttpPost("with-attributes")]
    public IActionResult LogEventWithAttributes([FromBody] CustomEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.EventName))
        {
            return BadRequest(new { error = "EventName is required" });
        }

        _customEventLogger.LogCustomEvent(request.EventName, request.UserId, request.IpAddress);
        return Ok(new { message = "Custom event with attributes logged successfully" });
    }

    /// <summary>
    /// Logs a custom event at a specific log level.
    /// </summary>
    /// <param name="request">The event details</param>
    /// <example>
    /// POST /api/custom-events/with-level
    /// Body: { "eventName": "critical-action", "logLevel": "Warning", "details": "Something needs attention" }
    /// Response: 200 OK
    /// </example>
    [HttpPost("with-level")]
    public IActionResult LogEventWithLevel([FromBody] CustomEventWithLevelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.EventName))
        {
            return BadRequest(new { error = "EventName is required" });
        }

        if (!Enum.TryParse<Microsoft.Extensions.Logging.LogLevel>(request.LogLevel, true, out var logLevel))
        {
            return BadRequest(new { error = "Invalid LogLevel. Allowed values: Trace, Debug, Information, Warning, Error, Critical, None" });
        }

        _customEventLogger.LogCustomEvent(request.EventName, logLevel, request.Details);
        return Ok(new { message = "Custom event with log level logged successfully" });
    }

    /// <summary>
    /// Logs a custom event with an exception.
    /// </summary>
    /// <param name="request">The event details</param>
    /// <example>
    /// POST /api/custom-events/with-exception
    /// Body: { "eventName": "error-occurred", "exceptionMessage": "Database connection failed" }
    /// Response: 200 OK
    /// </example>
    [HttpPost("with-exception")]
    public IActionResult LogEventWithException([FromBody] CustomEventWithExceptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.EventName))
        {
            return BadRequest(new { error = "EventName is required" });
        }

        var exception = new Exception(request.ExceptionMessage ?? "An error occurred");
        _customEventLogger.LogCustomEvent(request.EventName, exception, request.Details);
        return Ok(new { message = "Custom event with exception logged successfully" });
    }
}

/// <summary>
/// Request model for logging a custom event with attributes.
/// </summary>
public class CustomEventRequest
{
    public string? EventName { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
}

/// <summary>
/// Request model for logging a custom event with a specific log level.
/// </summary>
public class CustomEventWithLevelRequest
{
    public string? EventName { get; set; }
    public string? LogLevel { get; set; }
    public string? Details { get; set; }
}

/// <summary>
/// Request model for logging a custom event with an exception.
/// </summary>
public class CustomEventWithExceptionRequest
{
    public string? EventName { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? Details { get; set; }
}
