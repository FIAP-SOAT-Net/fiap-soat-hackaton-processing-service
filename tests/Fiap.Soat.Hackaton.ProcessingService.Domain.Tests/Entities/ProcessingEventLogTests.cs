using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Tests.Entities;

public class ProcessingEventLogTests
{
    [Test]
    public void Create_ShouldPopulateAllProperties()
    {
        var processingFileId = Guid.NewGuid();

        var sut = ProcessingEventLog.Create(processingFileId, "PROCESSING_STARTED", "RECEIVED", "PROCESSING");

        Assert.That(sut.ProcessingFileId, Is.EqualTo(processingFileId));
        Assert.That(sut.EventType, Is.EqualTo("PROCESSING_STARTED"));
        Assert.That(sut.StatusFrom, Is.EqualTo("RECEIVED"));
        Assert.That(sut.StatusTo, Is.EqualTo("PROCESSING"));
    }
}

