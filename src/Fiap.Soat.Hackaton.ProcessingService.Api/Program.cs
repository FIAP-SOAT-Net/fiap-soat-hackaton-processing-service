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

_ = builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionString, new MySqlServerVersion(new Version(8, 4, 0))));

_ = builder.Services.AddScoped<IProcessingFileRepository, ProcessingFileRepository>();
_ = builder.Services.AddScoped<IProcessingEventLogRepository, ProcessingEventLogRepository>();

// Add services to the container.
_ = builder.Services.AddServiceExtensions();
_ = builder.Services.AddRepositoryExtensions();
_ = builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
_ = builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateFileStatusCommand).Assembly));
_ = builder.Services.AddHttpContextAccessor();
_ = builder.Services.AddHealthChecks()
    .AddCheck<DetailedHealthCheck>("detailed")
    .AddDbContextCheck<AppDbContext>("database");
_ = builder.Services.AddRouting(options => options.LowercaseUrls = true);
_ = builder.Services.AddSwaggerExtension(builder.Configuration);
_ = builder.Services.AddMemoryCache();
_ = builder.Services.AddInterfaceAdapters();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
_ = builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

_ = app.UseHttpsRedirection();
_ = app.UseMiddleware<RequestLoggingEnrichmentMiddleware>();

_ = app.UseSwagger();
_ = app.UseSwaggerUI(c =>
{
    c.EnableTryItOutByDefault();
    c.DisplayRequestDuration();
});

_ = app.UseReDoc(c =>
{
    c.RoutePrefix = "docs";
    c.DocumentTitle = "Fiap | Soat | Hackaton | Processing File Api";
    c.SpecUrl = "/swagger/v1/swagger.json";
});
_ = app.UseMiddleware<ExceptionMiddleware>();
_ = app.UseHttpsRedirection();
_ = app.UseAuthorization();
_ = app.MapControllers();
_ = app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
