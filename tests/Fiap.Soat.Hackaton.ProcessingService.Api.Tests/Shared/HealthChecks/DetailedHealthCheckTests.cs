using Fiap.Soat.Hackaton.ProcessingService.Api.Shared.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.Tests.Shared.HealthChecks;

public class DetailedHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_ShouldReturnHealthyWithExpectedMetadata()
    {
        // Arrange
        var sut = new DetailedHealthCheck();
        var context = new HealthCheckContext();

        // Act
        var result = await sut.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
        Assert.That(result.Description, Is.EqualTo("API is healthy"));

        Assert.Multiple(() =>
        {
            Assert.That(result.Data.ContainsKey("version"), Is.True);
            Assert.That(result.Data.ContainsKey("timestamp"), Is.True);
            Assert.That(result.Data.ContainsKey("uptime"), Is.True);
            Assert.That(result.Data.ContainsKey("memoryUsedMB"), Is.True);
            Assert.That(result.Data.ContainsKey("gen0Collections"), Is.True);
            Assert.That(result.Data.ContainsKey("gen1Collections"), Is.True);
            Assert.That(result.Data.ContainsKey("gen2Collections"), Is.True);
        });

        Assert.That(result.Data["version"]?.ToString(), Is.Not.Null.And.Not.Empty);
        Assert.That(DateTimeOffset.TryParse(result.Data["timestamp"]?.ToString(), out _), Is.True);
        Assert.That(Convert.ToDouble(result.Data["uptime"]), Is.GreaterThanOrEqualTo(0));
    }
}

