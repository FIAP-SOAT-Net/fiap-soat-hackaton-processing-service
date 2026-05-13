using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.EventLog;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Update;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using MediatR;
using Moq;
using System.Net;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Tests.UseCases.File.Update;

public class UpdateProcessingFileStatusHandlerTests
{
    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenFileDoesNotExist()
    {
        var repository = new Mock<IProcessingFileRepository>();
        repository
            .Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProcessingFile?) null);

        var mediator = new Mock<IMediator>(MockBehavior.Strict);
        var sut = new UpdateProcessingFileStatusHandler(repository.Object, mediator.Object);

        var response = await sut.Handle(new UpdateProcessingFileStatusCommand(Guid.NewGuid(), "PROCESSING"), CancellationToken.None);

        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(response.Reasons.Select(reason => reason.Message), Contains.Item("Processing file not found"));
    }

    [Test]
    public async Task Handle_ShouldPublishEventAndUpdateEntity_WhenFileExists()
    {
        var entity = new ProcessingFile("file-1", "name", "bucket", "key", 10, DateTime.UtcNow, "text/plain");

        var repository = new Mock<IProcessingFileRepository>();
        repository
            .Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        repository
            .Setup(mock => mock.UpdateAsync(It.IsAny<ProcessingFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProcessingFile processingFile, CancellationToken _) => processingFile);

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(mock => mock.Publish(It.IsAny<CreateEventLogNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new UpdateProcessingFileStatusHandler(repository.Object, mediator.Object);

        var response = await sut.Handle(new UpdateProcessingFileStatusCommand(Guid.NewGuid(), "PROCESSING"), CancellationToken.None);

        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data.Status, Is.EqualTo("PROCESSING"));

        mediator.Verify(mock => mock.Publish(It.IsAny<CreateEventLogNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(mock => mock.UpdateAsync(It.IsAny<ProcessingFile>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
