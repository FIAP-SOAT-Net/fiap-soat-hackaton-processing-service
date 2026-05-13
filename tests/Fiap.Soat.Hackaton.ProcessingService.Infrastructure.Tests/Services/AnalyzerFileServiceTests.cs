using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.Services;

public class AnalyzerFileServiceTests
{
    [Test]
    public async Task SendAsync_ShouldPostPayloadToAnalyzerAndLogInformationOnSuccess()
    {
        var logger = new RecordingLogger<AnalyzerFileService>();
        var handler = new RecordingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var factory = new FixedHttpClientFactory(new HttpClient(handler));
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AnalyzerService:BaseUrl"] = "http://localhost:8080"
            })
            .Build();

        var sut = new AnalyzerFileService(logger, factory, configuration);
        var processingFile = new ProcessingFile("file-1", "document.pdf", "bucket-a", "uploads/document.pdf", 2048, DateTime.UtcNow, "application/pdf");

        await sut.SendAsync(processingFile);

        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest!.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Is.EqualTo("http://localhost:8080/analyze"));
        Assert.That(handler.LastRequest.Content!.Headers.ContentType!.MediaType, Is.EqualTo("application/json"));

        var payload = JsonDocument.Parse(handler.LastBody!);
        Assert.That(payload.RootElement.GetProperty("FileId").GetString(), Is.EqualTo("file-1"));
        Assert.That(payload.RootElement.GetProperty("Name").GetString(), Is.EqualTo("document.pdf"));

        Assert.That(logger.Entries, Has.Count.EqualTo(1));
        Assert.That(logger.Entries[0].Level, Is.EqualTo(LogLevel.Information));
        Assert.That(logger.Entries[0].Message, Does.Contain("Successfully sending processing file"));
    }

    [Test]
    public async Task SendAsync_ShouldLogError_WhenAnalyzerReturnsFailure()
    {
        var logger = new RecordingLogger<AnalyzerFileService>();
        var handler = new RecordingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.BadRequest));
        var factory = new FixedHttpClientFactory(new HttpClient(handler));
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AnalyzerService:BaseUrl"] = "http://localhost:8080"
            })
            .Build();

        var sut = new AnalyzerFileService(logger, factory, configuration);
        var processingFile = new ProcessingFile("file-1", "document.pdf", "bucket-a", "uploads/document.pdf", 2048, DateTime.UtcNow, "application/pdf");

        await sut.SendAsync(processingFile);

        Assert.That(logger.Entries, Has.Count.EqualTo(1));
        Assert.That(logger.Entries[0].Level, Is.EqualTo(LogLevel.Error));
        Assert.That(logger.Entries[0].Message, Does.Contain("Failed sending processing file"));
        Assert.That(logger.Entries[0].Message, Does.Contain("BadRequest"));
    }

    [Test]
    public void SendAsync_ShouldThrow_WhenAnalyzerBaseUrlIsMissing()
    {
        var logger = new RecordingLogger<AnalyzerFileService>();
        var factory = new FixedHttpClientFactory(new HttpClient(new RecordingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK))));
        var configuration = new ConfigurationBuilder().Build();
        var sut = new AnalyzerFileService(logger, factory, configuration);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.SendAsync(new ProcessingFile("file-1", "name", "bucket", "key", 1, DateTime.UtcNow, null)));

        Assert.That(ex!.Message, Is.EqualTo("AnalyzerService:BaseUrl not configured"));
        Assert.That(factory.CreateClientCalls, Is.EqualTo(1));
    }

    private sealed class FixedHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public int CreateClientCalls { get; private set; }

        public HttpClient CreateClient(string name)
        {
            CreateClientCalls++;
            return client;
        }
    }

    private sealed class RecordingHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content is not null)
            {
                LastBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return response;
        }
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}

