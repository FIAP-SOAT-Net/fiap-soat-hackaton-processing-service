using AnalyzerService.Api;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

_ = builder.Services.AddOpenApi();
_ = builder.Services.AddSingleton(typeof(ILogger), typeof(Logger<Program>));
_ = builder.Services.AddScoped<IUpdateProcessingFileService, UpdateProcessingFileService>();
_ = builder.Services.AddScoped<ISendAnalysisResultService, SendAnalysisResultService>();

_ = builder.Services.AddHttpClient();

_ = builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());

// Add the processing server as an IHostedService.
_ = builder.Services.AddHangfireServer();
_ = builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

_ = builder.Services.AddHttpLogging();

var app = builder.Build();

_ = app.UseCors("AllowAll");
_ = app.UseHttpLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

_ = app.UseHttpsRedirection();

_ = app.MapPost("/analyze", async (
        [FromBody] ProcessingFileDto request,
        [FromServices] ILogger logger,
        [FromServices] IBackgroundJobClient backgroundJobs,
        [FromServices] IUpdateProcessingFileService fileService,
        [FromServices] ISendAnalysisResultService reportService
        ) =>
    {
        logger.LogInformation("Start creating analysis to {@Request}", request);
        var firstProcessingJobId = BackgroundJob.Schedule(() => fileService.UpdateProcessingFileAsync(request.Id, "PROCESSING"), TimeSpan.FromSeconds(3));
        var secondProcessingJobId = BackgroundJob.ContinueJobWith(firstProcessingJobId, () => fileService.UpdateProcessingFileAsync(request.Id, "PROCESSING"));
        var analyzedJobId = BackgroundJob.ContinueJobWith(secondProcessingJobId, () => fileService.UpdateProcessingFileAsync(request.Id, "ANALYZED"));
        var processedJobId = BackgroundJob.ContinueJobWith(analyzedJobId, () => fileService.UpdateProcessingFileAsync(request.Id, "PROCESSED"));
        _ = BackgroundJob.ContinueJobWith(processedJobId, () => reportService.SendAsync(request.Id, request.Name));
        return Results.Created();
    })
    .WithName("SendAnalysisRequest");

_ = app.UseHangfireDashboard();

await app.RunAsync();

internal partial class Program { }
