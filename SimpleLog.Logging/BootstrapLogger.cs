using Serilog;

namespace SimpleLog.Logging;

/// <summary>
/// Wrapper for Serilog bootstrap logger lifecycle.
/// </summary>
public static class BootstrapLogger
{
    /// <summary>
    /// Initializes the bootstrap logger for early startup logging.
    /// </summary>
    public static void Initialize()
    {
        Log.Logger = Logging.Extensions.SerilogServiceExtensions.CreateBootstrapLogger();
    }

    /// <summary>
    /// Logs an informational startup message.
    /// </summary>
    public static void Information(string message)
    {
        Log.Information(message);
    }

    /// <summary>
    /// Logs a fatal startup error.
    /// </summary>
    public static void Fatal(Exception exception, string message)
    {
        Log.Fatal(exception, message);
    }

    /// <summary>
    /// Flushes and closes the logger.
    /// </summary>
    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}
