using Microsoft.AspNetCore.Mvc;
using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Api.Controllers;

[ApiController]
[Route("api/workflow")]
public class WorkflowController : ControllerBase
{
    private readonly IAppInsightsLogger _logger;

    public WorkflowController(IAppInsightsLogger logger)
    {
        _logger = logger;
    }

    [HttpPost("approve")]
    public IActionResult Approve([FromBody] WorkflowActionRequest request)
    {
        return LogWorkflowAction(request, "approve");
    }

    [HttpPost("sendback")]
    public IActionResult SendBack([FromBody] WorkflowActionRequest request)
    {
        return LogWorkflowAction(request, "sendback");
    }

    [HttpPost("reject")]
    public IActionResult Reject([FromBody] WorkflowActionRequest request)
    {
        return LogWorkflowAction(request, "reject");
    }

    private IActionResult LogWorkflowAction(WorkflowActionRequest request, string action)
    {
        if (request is null)
        {
            return BadRequest("Request body is required.");
        }

        if (string.IsNullOrWhiteSpace(request.UserId) ||
            string.IsNullOrWhiteSpace(request.Role) ||
            string.IsNullOrWhiteSpace(request.Permission) ||
            string.IsNullOrWhiteSpace(request.WorkflowId) ||
            string.IsNullOrWhiteSpace(request.WorkflowStateId))
        {
            return BadRequest("All fields are required: userId, role, permission, workflowId, workflowStateId.");
        }

        var properties = new Dictionary<string, string>
        {
            { "userId", request.UserId },
            { "action", action },
            { "role", request.Role },
            { "permission", request.Permission },
            { "workflowId", request.WorkflowId },
            { "workflowstateid", request.WorkflowStateId }
        };

        if (request.Properties is not null)
        {
            foreach (var kvp in request.Properties)
            {
                properties.TryAdd(kvp.Key, kvp.Value);
            }
        }

        _logger.TrackEvent("WorkflowAction", properties);
        _logger.LogInformation("Workflow action logged: {Action} for workflow {WorkflowId}", action, request.WorkflowId);

        return Ok(new
        {
            status = "logged",
            action,
            request.UserId,
            request.WorkflowId,
            request.WorkflowStateId
        });
    }
}

public class WorkflowActionRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public string WorkflowId { get; set; } = string.Empty;
    public string WorkflowStateId { get; set; } = string.Empty;
    public Dictionary<string, string>? Properties { get; set; }
}
