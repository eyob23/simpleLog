using Microsoft.AspNetCore.Mvc;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IAzureMonitorLogger _logger;

    public HealthController(IAzureMonitorLogger logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Health check endpoint called");

        _logger.TrackEvent("HealthCheckPerformed", new Dictionary<string, string>
        {
            { "Status", "Healthy" },
            { "Timestamp", DateTime.UtcNow.ToString("O") }
        });

        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "SimpleLog.Api"
        });
    }

    [HttpGet("detailed")]
    public IActionResult GetDetailed()
    {
        _logger.LogDebug("Detailed health check endpoint called");

        var healthData = new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "SimpleLog.Api",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            uptime = Environment.TickCount64 / 1000.0 // seconds
        };

        _logger.TrackMetric("ServiceUptime", healthData.uptime);

        return Ok(healthData);
    }
}
