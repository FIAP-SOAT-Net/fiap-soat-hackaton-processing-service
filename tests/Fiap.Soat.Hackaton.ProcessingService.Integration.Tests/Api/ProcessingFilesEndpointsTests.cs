using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;
using Fiap.Soat.Hackaton.ProcessingService.Integration.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.Soat.Hackaton.ProcessingService.Integration.Tests.Api;

public class ProcessingFilesEndpointsTests
{
    private ProcessingServiceApiFactory _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _factory = new ProcessingServiceApiFactory();
        _client = _factory.CreateHttpsClient();
        await _factory.ResetDatabaseAsync();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        await _factory.ResetDatabaseAsync();
    }

    [Test]
    public async Task GetHealth_ShouldReturnHealthyStatus()
    {
        var response = await _client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(body, Does.Contain("Healthy"));
    }

    [Test]
    public async Task GetProcessingFiles_ShouldReturnSeededFileInList()
    {
        var seeded = await SeedProcessingFileAsync("file-1", "RECEIVED");
        await SeedProcessingFileAsync("file-2", "PROCESSING");

        var response = await _client.GetAsync("/processingFiles?status=RECEIVED");
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var json = JsonDocument.Parse(body);
        var payload = GetProperty(json.RootElement, "value");
        var data = GetProperty(payload, "data");
        Assert.That(data.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(data.EnumerateArray().Any(item => GetProperty(item, "fileId").GetString() == seeded.FileId), Is.True);
        Assert.That(data.EnumerateArray().All(item => GetProperty(item, "status").GetString() == "RECEIVED"), Is.True);
    }

    [Test]
    public async Task GetProcessingFileById_ShouldReturnSeededFile()
    {
        var seeded = await SeedProcessingFileAsync("file-1", "RECEIVED");

        var response = await _client.GetAsync($"/processingFiles/{seeded.Id}");
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var json = JsonDocument.Parse(body);
        var payload = GetProperty(json.RootElement, "value");
        Assert.That(GetProperty(payload, "isSuccess").GetBoolean(), Is.True);
        Assert.That(GetProperty(payload, "data").GetProperty("fileId").GetString(), Is.EqualTo("file-1"));
        Assert.That(GetProperty(payload, "data").GetProperty("status").GetString(), Is.EqualTo("RECEIVED"));
    }

    [Test]
    public async Task PatchProcessingFile_ShouldUpdateStatusAndPersistChange()
    {
        var seeded = await SeedProcessingFileAsync("file-1", "RECEIVED");
        var payload = JsonContent.Create(new { status = "PROCESSING" });

        var response = await _client.PatchAsync($"/processingFiles/{seeded.Id}", payload);
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using var json = JsonDocument.Parse(body);
        var responsePayload = GetProperty(json.RootElement, "value");
        Assert.That(GetProperty(responsePayload, "isSuccess").GetBoolean(), Is.True);
        Assert.That(GetProperty(responsePayload, "data").GetProperty("status").GetString(), Is.EqualTo("PROCESSING"));

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = await context.ProcessingFiles.SingleAsync(x => x.Id == seeded.Id);
        Assert.That(persisted.Status, Is.EqualTo("PROCESSING"));
    }

    private async Task<ProcessingFile> SeedProcessingFileAsync(string fileId, string status)
    {
        var entity = new ProcessingFile(fileId, $"{fileId}.mp4", "bucket-a", $"uploads/{fileId}.mp4", 1024, DateTime.UtcNow, "video/mp4");
        entity.UpdateStatus(status);
        await _factory.SeedProcessingFileAsync(entity);
        return entity;
    }

    private static JsonElement GetProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var value))
        {
            return value;
        }

        var camelCase = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
        if (element.TryGetProperty(camelCase, out value))
        {
            return value;
        }

        var pascalCase = char.ToUpperInvariant(propertyName[0]) + propertyName[1..];
        if (element.TryGetProperty(pascalCase, out value))
        {
            return value;
        }

        throw new AssertionException($"Property '{propertyName}' was not found in JSON payload.");
    }
}

