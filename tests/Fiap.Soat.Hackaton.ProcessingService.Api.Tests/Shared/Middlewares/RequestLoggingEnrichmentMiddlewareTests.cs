using Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Middlewares;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.Tests.Shared.Middlewares;

public class RequestLoggingEnrichmentMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_ShouldUseCorrelationHeaderAndTracingHeadersInLogContext()
    {
        var events = new List<LogEvent>();
        var sink = new InMemorySink(events);
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(sink)
            .CreateLogger();

        var originalLogger = Log.Logger;
        Log.Logger = logger;

        try
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/processingFiles";
            context.Request.Method = "GET";
            context.Request.Headers["X-Correlation-ID"] = "corr-123";
            context.Request.Headers["traceparent"] = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-00";
            context.Request.Headers["newrelic"] = "newrelic-data";

            var sut = new RequestLoggingEnrichmentMiddleware(_ =>
            {
                Log.Information("request received");
                return Task.CompletedTask;
            });

            await sut.InvokeAsync(context);

            Assert.That(events.Count, Is.EqualTo(1));
            var logEvent = events[0];

            Assert.Multiple(() =>
            {
                Assert.That(GetScalarValue(logEvent, "CorrelationId"), Is.EqualTo("corr-123"));
                Assert.That(GetScalarValue(logEvent, "RequestPath"), Is.EqualTo("/processingFiles"));
                Assert.That(GetScalarValue(logEvent, "RequestMethod"), Is.EqualTo("GET"));
                Assert.That(GetScalarValue(logEvent, "TraceParent"), Is.EqualTo("00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-00"));
                Assert.That(GetScalarValue(logEvent, "NewRelicTrace"), Is.EqualTo("newrelic-data"));
            });
        }
        finally
        {
            Log.Logger = originalLogger;
            logger.Dispose();
        }
    }

    [Test]
    public async Task InvokeAsync_ShouldFallbackToTraceIdentifier_WhenCorrelationHeaderIsMissing()
    {
        var events = new List<LogEvent>();
        var sink = new InMemorySink(events);
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(sink)
            .CreateLogger();

        var originalLogger = Log.Logger;
        Log.Logger = logger;

        try
        {
            var context = new DefaultHttpContext
            {
                TraceIdentifier = "trace-id-001"
            };

            var sut = new RequestLoggingEnrichmentMiddleware(_ =>
            {
                Log.Information("request received");
                return Task.CompletedTask;
            });

            await sut.InvokeAsync(context);

            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(GetScalarValue(events[0], "CorrelationId"), Is.EqualTo("trace-id-001"));
        }
        finally
        {
            Log.Logger = originalLogger;
            logger.Dispose();
        }
    }

    private static string? GetScalarValue(LogEvent logEvent, string propertyName)
    {
        return logEvent.Properties.TryGetValue(propertyName, out var value)
            ? (value as ScalarValue)?.Value?.ToString()
            : null;
    }

    private sealed class InMemorySink(List<LogEvent> events) : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            events.Add(logEvent);
        }
    }
}

