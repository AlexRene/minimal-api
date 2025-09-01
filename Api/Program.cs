using MinimalApi;
using Serilog;
using Serilog.Events;

try
{
    Log.Information("Iniciando aplicação Minimal API");

    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/minimal-api-.log", 
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));

    var app = builder.Build();

    // Configurar o pipeline de requisições
    app.UseMiddleware<MinimalApi.Middleware.RequestLoggingMiddleware>();
    app.UseMiddleware<MinimalApi.Middleware.GlobalExceptionHandlerMiddleware>();

    // Configurar endpoints
    app.MapGet("/", () => "Minimal API está funcionando!");

    Log.Information("Aplicação Minimal API iniciada com sucesso");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}