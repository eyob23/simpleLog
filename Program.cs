using SimpleLog.Logging;
using SimpleLog.Logging.Extensions;

// Configure bootstrap logger for startup
BootstrapLogger.Initialize();

try
{
    BootstrapLogger.Information("Starting SimpleLog API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog with Application Insights integration from Logging namespace
    builder.Host.AddSerilogLogging(builder.Configuration);

    // Add services to the container
    builder.Services.AddControllers();

    // Configure Azure Monitor OpenTelemetry logging
    builder.Services.AddAzureMonitorOpenTelemetryLogging(builder.Configuration);

    // Add Swagger/OpenAPI support
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    BootstrapLogger.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    BootstrapLogger.CloseAndFlush();
}
