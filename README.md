# SimpleLog API

A .NET 8 Web API with Azure Application Insights logging functionality designed for easy extraction into a separate NuGet package.

## Project Structure

```
SimpleLog/
├── Logging/                          # Future NuGet package - SimpleLog.Logging
│   ├── Interfaces/
│   │   └── IAppInsightsLogger.cs    # Logger interface
│   ├── Implementation/
│   │   └── AppInsightsLogger.cs     # Logger implementation
│   └── Extensions/
│       └── LoggingServiceExtensions.cs  # DI registration extensions
├── Controllers/                      # API Controllers
│   ├── WeatherForecastController.cs
│   └── HealthController.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs                        # Application entry point
├── SimpleLog.Api.csproj
├── appsettings.json
└── appsettings.Development.json
```

## Features

- **Structured Logging**: Comprehensive logging with different severity levels
- **Application Insights Integration**: Full telemetry tracking including:
  - Custom events
  - Metrics
  - Dependencies
  - Exceptions
- **Separate Namespace**: Logging functionality isolated in `SimpleLog.Logging` namespace for easy NuGet packaging
- **Dependency Injection**: Clean DI registration through extension methods
- **Sample Controllers**: Demonstrates logging usage patterns

## Setup

### 1. Configure Application Insights

Update the `ConnectionString` in `appsettings.json`:

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key;IngestionEndpoint=https://your-region.in.applicationinsights.azure.com/"
  }
}
```

To get your connection string:
1. Create an Application Insights resource in Azure Portal
2. Copy the connection string from the Overview page

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: https://localhost:5001/swagger

## Usage

### Using the Logger in Your Controllers

```csharp
public class YourController : ControllerBase
{
    private readonly IAppInsightsLogger _logger;

    public YourController(IAppInsightsLogger logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        // Log information
        _logger.LogInformation("Processing request");

        // Track custom events
        _logger.TrackEvent("CustomEvent", new Dictionary<string, string>
        {
            { "Key", "Value" }
        });

        // Track metrics
        _logger.TrackMetric("ResponseTime", 123.45);

        return Ok();
    }
}
```

## API Endpoints

- `GET /api/weather` - Get weather forecast (demonstrates logging, events, metrics)
- `GET /api/weather/{id}` - Get specific forecast by ID
- `POST /api/weather/simulate-error` - Simulate error for testing exception logging
- `GET /api/health` - Basic health check
- `GET /api/health/detailed` - Detailed health information

## Creating the NuGet Package

When ready to extract the logging functionality into a NuGet package:

### 1. Create a new project

```bash
dotnet new classlib -n SimpleLog.Logging
```

### 2. Move the Logging folder contents

Move these files to the new project:
- `Logging/Interfaces/IAppInsightsLogger.cs`
- `Logging/Implementation/AppInsightsLogger.cs`
- `Logging/Extensions/LoggingServiceExtensions.cs`

### 3. Update the .csproj for packaging

```xml
<PropertyGroup>
  <PackageId>SimpleLog.Logging</PackageId>
  <Version>1.0.0</Version>
  <Authors>Your Name</Authors>
  <Description>Application Insights logging wrapper for .NET applications</Description>
  <PackageTags>logging;applicationinsights;telemetry</PackageTags>
</PropertyGroup>
```

### 4. Build the NuGet package

```bash
dotnet pack -c Release
```

## Dependencies

- Microsoft.ApplicationInsights.AspNetCore (2.22.0)
- Microsoft.Extensions.Logging.ApplicationInsights (2.22.0)
- Swashbuckle.AspNetCore (6.5.0)

## License

MIT
