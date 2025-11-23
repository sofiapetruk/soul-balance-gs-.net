using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using soulBalanceGs.Data;
using soulBalanceGs.DTOs;
using soulBalanceGs.Repository;
using soulBalanceGs.Service;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddScoped<IService<UsuarioResponseDto, UsuarioRequestDto>, UsuarioService>();
builder.Services.AddScoped<IService<CheckinManualResponseDto, CheckinManualRequestDto>, CheckinManualService>();
builder.Services.AddScoped<CheckinManualService>();
builder.Services.AddScoped<IService<AtividadeResponseDto, AtividadeRequestDto>, AtividadeService>();
builder.Services.AddScoped<AtividadeService>();
builder.Services.AddScoped<ICheckinManualRepository, CheckinManualRepository>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API OK"))
    .AddOracle(
        builder.Configuration.GetConnectionString("DefaultConnection"),
    name: "OracleDB-Check",
    tags: new[] { "database", "oracle" }
);

builder.Services.AddHealthChecksUI(setupSettings =>
{
    setupSettings.AddHealthCheckEndpoint("Minha API", "/health");
    setupSettings.SetEvaluationTimeInSeconds(60);
})
.AddInMemoryStorage();


builder.Services.AddOpenTelemetry()
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("soulBalanceGs"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddJaegerExporter(o =>
            {
                o.AgentHost = builder.Configuration["Jaeger:Host"] ?? "localhost";
                o.AgentPort = int.TryParse(builder.Configuration["Jaeger:Port"], out var port) ? port : 6831;
            })
            .AddConsoleExporter();
    });

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});


var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/ping", () => Results.Ok("pong"));

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.MapControllers();

app.Run();