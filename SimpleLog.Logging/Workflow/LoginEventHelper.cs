using SimpleLog.Logging.Interfaces;

namespace SimpleLog.Logging.Workflow;

/// <summary>
/// Helper for logging user login events with required and optional properties.
/// </summary>
public static class LoginEventHelper
{
    /// <summary>
    /// Tracks a user login action as a custom event with required and optional properties.
    /// </summary>
    public static bool TryTrackLogin(
        IAppInsightsLogger logger,
        string? userId,
        bool success,
        string? ipAddress,
        string? userAgent,
        string? method,
        IDictionary<string, string>? extraProperties,
        out string? error)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            error = "userId is required.";
            return false;
        }

        var properties = new Dictionary<string, string>
        {
            { "userId", userId },
            { "success", success.ToString() },
            { "ipAddress", ipAddress ?? string.Empty },
            { "userAgent", userAgent ?? string.Empty },
            { "method", method ?? string.Empty }
        };

        if (extraProperties is not null)
        {
            foreach (var kvp in extraProperties)
            {
                properties.TryAdd(kvp.Key, kvp.Value);
            }
        }

        logger.TrackEvent("UserLogin", properties);
        error = null;
        return true;
    }
}
