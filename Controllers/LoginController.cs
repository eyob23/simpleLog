using Microsoft.AspNetCore.Mvc;
using SimpleLog.Logging.Interfaces;
using SimpleLog.Logging.Workflow;

namespace SimpleLog.Api.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly IAzureMonitorLogger _logger;

    public LoginController(IAzureMonitorLogger logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request is null)
        {
            return BadRequest("Request body is required.");
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return BadRequest("userId is required.");
        }

        if (!LoginEventHelper.TryTrackLogin(
                _logger,
                request.UserId,
                request.Success,
                request.IpAddress,
                request.UserAgent,
                request.Method,
                request.Properties,
                out var error))
        {
            return BadRequest(error);
        }
        _logger.LogInformation("User login tracked for {UserId} (success: {Success})", request.UserId, request.Success);

        return Ok(new
        {
            status = "logged",
            request.UserId,
            request.Success
        });
    }
}

public class LoginRequest
{
    public string UserId { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Method { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
}
