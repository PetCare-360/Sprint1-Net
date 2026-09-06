using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetCare360.Data;
using PetCare360.Middleware;
using PetCare360.Repositories.Implementations;
using PetCare360.Repositories.Interfaces;
using PetCare360.Services;
using PetCare360.Services.Interfaces;
using Serilog;
using PetCare360.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/petcare360-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "PetCare360")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));

if (builder.Environment.IsEnvironment("Testing"))
{
    var databaseName =
        builder.Configuration["Testing:DatabaseName"]
        ?? "PetCare360IntegrationTests";

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase(databaseName));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(
            builder.Configuration.GetConnectionString("OracleConnection")));
}

builder.Services.AddHttpClient();

var jwtSecret = builder.Configuration["Jwt:Secret"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        "oracle",
        tags: new[] { "ready" })
    .AddCheck<ExternalServiceHealthCheck>(
        "external-service",
        tags: new[] { "ready" });

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("PetCare360"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter("PetCare360")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<ISensorDataRepository, SensorDataRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();

builder.Services.AddScoped<PetMapper>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();
builder.Services.AddScoped<IIotProcessingService, IotProcessingService>();

builder.Services.AddSingleton<ApplicationMetrics>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PetCare360 API",
        Version = "v1",
        Description =
            "API de monitoramento contínuo da saúde de pets — FIAP Challenge 2026"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
            return Serilog.Events.LogEventLevel.Error;

        if (httpContext.Response.StatusCode >= 400)
            return Serilog.Events.LogEventLevel.Warning;

        return Serilog.Events.LogEventLevel.Information;
    };

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
        diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
    };
});

app.UseMiddleware<RequestMetricsMiddleware>();

app.MapHealthChecks("/health");

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapPrometheusScrapingEndpoint("/metrics");

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "PetCare360 API v1");

    c.RoutePrefix = string.Empty;
});

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }