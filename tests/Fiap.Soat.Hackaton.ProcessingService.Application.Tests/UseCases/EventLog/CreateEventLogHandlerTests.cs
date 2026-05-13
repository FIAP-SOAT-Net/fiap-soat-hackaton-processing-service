using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.EventLog;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Moq;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Tests.UseCases.EventLog;

public class CreateEventLogHandlerTests
{
    [TestCase(null, "RECEIVED", "FILE_RECEIVED")]
    [TestCase("RECEIVED", "PROCESSING", "PROCESSING_STARTED")]
    [TestCase("PROCESSING", "PROCESSING", "EXTERNAL_REQUEST_SENT")]
    [TestCase("PROCESSING", "ANALYZED", "ANALYSIS_COMPLETED")]
    [TestCase("ANALYZED", "PROCESSED", "PROCESSING_FINALIZED")]
    [TestCase("ANY", "ERROR", "PROCESSING_FAILED")]
    [TestCase("RECEIVED", "PROCESSED", "FILE_STATUS_UPDATED")]
    public async Task Handle_ShouldCreateExpectedEventType(string? statusFrom, string statusTo, string expectedEventType)
    {
        ProcessingEventLog? capturedEntity = null;
        var repository = new Mock<IProcessingEventLogRepository>();
        repository
            .Setup(mock => mock.AddAsync(It.IsAny<ProcessingEventLog>(), It.IsAny<CancellationToken>()))
            .Callback<ProcessingEventLog, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync((ProcessingEventLog entity, CancellationToken _) => entity);

        var sut = new CreateEventLogHandler(repository.Object);
        var notification = new CreateEventLogNotification(Guid.NewGuid(), statusFrom, statusTo);

        await sut.Handle(notification, CancellationToken.None);

        Assert.That(capturedEntity, Is.Not.Null);
        Assert.That(capturedEntity!.EventType, Is.EqualTo(expectedEventType));
        Assert.That(capturedEntity.StatusFrom, Is.EqualTo(statusFrom));
        Assert.That(capturedEntity.StatusTo, Is.EqualTo(statusTo));
    }
}
