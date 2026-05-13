using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Services;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services;

public sealed class AnalyzerFileService(
    ILogger<AnalyzerFileService> logger,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : IAnalyzerFileService
{
    public async Task SendAsync(ProcessingFile processingFile)
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(configuration["AnalyzerService:BaseUrl"] ?? throw new InvalidOperationException("AnalyzerService:BaseUrl not configured"));
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(processingFile), System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("/analyze", content);
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Successfully sending processing file {@ProcessingFile} to be analyzed", processingFile);
        }
        else
        {
            logger.LogError("Failed sending processing file {@ProcessingFile} to be analyzed. Status code: {StatusCode}", processingFile, response.StatusCode);
        }
    }
}
