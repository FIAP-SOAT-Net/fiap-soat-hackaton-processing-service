using Fiap.Soat.Hackaton.ProcessingService.Api.BackgroundServices;
using Fiap.Soat.Hackaton.ProcessingService.Api.Endpoints;
using Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Extensions;
using Fiap.Soat.Hackaton.ProcessingService.Api.Shared.HealthChecks;
using Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Middlewares;
using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Services;
using Fiap.Soat.Hackaton.ProcessingService.Application.Mappers;
using Fiap.Soat.Hackaton.ProcessingService.Application.Shared;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Update;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services.Messaging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

_ = builder.Services.AddControllers();
_ = builder.Services.AddEndpointsApiExplorer();
_ = builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Configure RabbitMQ
_ = builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
_ = builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

// Configure MySQL
var mySqlConnectionString = builder.Configuration.GetConnectionString("MySql")
    ?? throw new InvalidOperationException("Connection string 'MySql' was not configured.");
_ = builder.Services.AddDbContext<AppDbContext>((options) => options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));

// Add services to the container.
_ = builder.Services.AddServiceExtensions();
_ = builder.Services.AddRepositoryExtensions();
_ = builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
_ = builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateProcessingFileStatusCommand).Assembly));
_ = builder.Services.AddHttpContextAccessor();
_ = builder.Services.AddHealthChecks()
    .AddCheck<DetailedHealthCheck>("detailed")
    .AddDbContextCheck<AppDbContext>("database");
_ = builder.Services.AddRouting(options => options.LowercaseUrls = true);
_ = builder.Services.AddMemoryCache();
_ = builder.Services.AddInterfaceAdapters();
_ = builder.Services.AddHostedService<MessagesConsumer>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
_ = builder.Services.AddOpenApi();

_ = builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

_ = app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

_ = app.UseHttpsRedirection();
_ = app.UseMiddleware<RequestLoggingEnrichmentMiddleware>();

_ = app.MapScalarApiReference();
_ = app.UseMiddleware<ExceptionMiddleware>();
_ = app.UseHttpsRedirection();
_ = app.UseAuthorization();
_ = app.MapControllers();
_ = app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

_ = ProcessingFileEndpoints.Map(app);

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
