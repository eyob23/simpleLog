using Microsoft.AspNetCore.Mvc;
using SimpleLog.Logging.Interfaces;
using SimpleLog.Logging.Workflow;

namespace SimpleLog.Api.Controllers;

[ApiController]
[Route("api/workflow")]
public class WorkflowController : ControllerBase
{
    private readonly IAzureMonitorLogger _logger;

    public WorkflowController(IAzureMonitorLogger logger)
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

        if (!WorkflowEventHelper.TryTrackWorkflowAction(
                _logger,
                request.UserId,
                action,
                request.Role,
                request.Permission,
                request.WorkflowId,
                request.WorkflowStateId,
                request.Properties,
                out var error))
        {
            return BadRequest(error);
        }
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
