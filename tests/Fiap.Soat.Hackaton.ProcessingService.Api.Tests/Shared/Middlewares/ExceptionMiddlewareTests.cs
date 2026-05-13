using System.Text.Json;
using Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Exceptions;
using Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
namespace Fiap.Soat.Hackaton.ProcessingService.Api.Tests.Shared.Middlewares;
public class ExceptionMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionIsThrown()
    {
        var wasCalled = false;
        RequestDelegate next = _ =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        };
        var sut = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = CreateHttpContext();
        await sut.InvokeAsync(context);
        Assert.That(wasCalled, Is.True);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }
    [Test]
    public async Task InvokeAsync_ShouldReturnBadRequest_WhenArgumentNullExceptionIsThrown()
    {
        RequestDelegate next = _ => throw new ArgumentNullException("fileId", "fileId is required");
        var sut = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = CreateHttpContext();
        await sut.InvokeAsync(context);
        var body = await ReadBodyAsync(context);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
        using var json = JsonDocument.Parse(body);
        Assert.That(GetProperty(json.RootElement, "IsSuccess").GetBoolean(), Is.False);
        Assert.That(GetProperty(json.RootElement, "Reasons").ValueKind, Is.EqualTo(JsonValueKind.Array));
    }
    [Test]
    public async Task InvokeAsync_ShouldReturnNotFoundPayload_WhenResourceNotFoundExceptionIsThrown()
    {
        const string message = "processing file not found";
        RequestDelegate next = _ => throw new ResourceNotFoundException(message);
        var sut = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = CreateHttpContext();
        await sut.InvokeAsync(context);
        var body = await ReadBodyAsync(context);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        using var json = JsonDocument.Parse(body);
        Assert.That(GetProperty(json.RootElement, "statusCode").GetInt32(), Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(GetProperty(json.RootElement, "message").GetString(), Is.EqualTo(message));
        Assert.That(GetProperty(json.RootElement, "errorType").GetString(), Is.EqualTo(nameof(ResourceNotFoundException)));
    }
    [Test]
    public async Task InvokeAsync_ShouldReturnInternalServerError_WhenUnhandledExceptionIsThrown()
    {
        const string message = "unexpected failure";
        RequestDelegate next = _ => throw new InvalidOperationException(message);
        var sut = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = CreateHttpContext();
        await sut.InvokeAsync(context);
        var body = await ReadBodyAsync(context);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        using var json = JsonDocument.Parse(body);
        Assert.That(GetProperty(json.RootElement, "statusCode").GetInt32(), Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(GetProperty(json.RootElement, "message").GetString(), Is.EqualTo(message));
        Assert.That(GetProperty(json.RootElement, "errorType").GetString(), Is.EqualTo(nameof(InvalidOperationException)));
    }
    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }
    private static async Task<string> ReadBodyAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }
    private static JsonElement GetProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var value))
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
