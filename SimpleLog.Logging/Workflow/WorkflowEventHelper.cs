using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Logging.Workflow;

/// <summary>
/// Helper for logging workflow actions with required and optional properties.
/// </summary>
public static class WorkflowEventHelper
{
    /// <summary>
    /// Tracks a workflow action as a custom event with required and optional properties.
    /// </summary>
    public static bool TryTrackWorkflowAction(
        IAppInsightsLogger logger,
        string? userId,
        string? action,
        string? role,
        string? permission,
        string? workflowId,
        string? workflowStateId,
        IDictionary<string, string>? extraProperties,
        out string? error)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(action) ||
            string.IsNullOrWhiteSpace(role) ||
            string.IsNullOrWhiteSpace(permission) ||
            string.IsNullOrWhiteSpace(workflowId) ||
            string.IsNullOrWhiteSpace(workflowStateId))
        {
            error = "All fields are required: userId, action, role, permission, workflowId, workflowStateId.";
            return false;
        }

        var properties = new Dictionary<string, string>
        {
            { "userId", userId },
            { "action", action },
            { "role", role },
            { "permission", permission },
            { "workflowId", workflowId },
            { "workflowstateid", workflowStateId }
        };

        if (extraProperties is not null)
        {
            foreach (var kvp in extraProperties)
            {
                properties.TryAdd(kvp.Key, kvp.Value);
            }
        }

        logger.TrackEvent("WorkflowAction", properties);
        error = null;
        return true;
    }
}
