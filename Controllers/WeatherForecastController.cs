using Microsoft.AspNetCore.Mvc;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IAppInsightsLogger _logger;

    public WeatherForecastController(IAppInsightsLogger logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("WeatherForecast endpoint called at {Time}", DateTime.UtcNow);

        // Track a custom event
        _logger.TrackEvent("WeatherForecastRequested", new Dictionary<string, string>
        {
            { "Endpoint", "GetWeatherForecast" },
            { "RequestTime", DateTime.UtcNow.ToString("O") }
        });

        var startTime = DateTimeOffset.UtcNow;

        try
        {
            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            var duration = DateTimeOffset.UtcNow - startTime;

            // Track successful operation
            _logger.TrackDependency("InMemory", "GenerateWeatherData", "Generate forecast data", startTime, duration, true);
            _logger.TrackMetric("WeatherForecastGenerated", forecast.Length);

            _logger.LogInformation("Successfully generated {Count} weather forecasts", forecast.Length);

            return forecast;
        }
        catch (Exception ex)
        {
            var duration = DateTimeOffset.UtcNow - startTime;
            _logger.TrackDependency("InMemory", "GenerateWeatherData", "Generate forecast data", startTime, duration, false);
            _logger.LogError("Error generating weather forecast", ex);
            throw;
        }
    }

    [HttpGet("{id}")]
    public ActionResult<WeatherForecast> GetById(int id)
    {
        _logger.LogDebug("Getting weather forecast by id: {Id}", id);

        if (id < 1 || id > 5)
        {
            _logger.LogWarning("Invalid weather forecast id requested: {Id}", id);
            return NotFound();
        }

        var forecast = new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(id)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };

        return Ok(forecast);
    }

    [HttpPost("simulate-error")]
    public IActionResult SimulateError()
    {
        try
        {
            _logger.LogInformation("Simulating an error condition");
            throw new InvalidOperationException("This is a simulated error for testing Application Insights");
        }
        catch (Exception ex)
        {
            _logger.LogError("Simulated error occurred", ex);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}
