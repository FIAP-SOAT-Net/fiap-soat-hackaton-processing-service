using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Services;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.EventLog;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Create;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using MediatR;
using Moq;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Tests.UseCases.File.Create;

public class RegisterUploadedFileHandlerTests
{
    [Test]
    public async Task Handle_ShouldReturnFailure_WhenFileIdIsEmpty()
    {
        var repository = new Mock<IProcessingFileRepository>(MockBehavior.Strict);
        var analyzer = new Mock<IAnalyzerFileService>(MockBehavior.Strict);
        var mediator = new Mock<IMediator>(MockBehavior.Strict);
        var sut = new RegisterUploadedFileHandler(repository.Object, analyzer.Object, mediator.Object);

        var command = CreateCommand(fileId: string.Empty);

        var response = await sut.Handle(command, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Reasons.Select(reason => reason.Message), Contains.Item("FileId is required"));
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenSizeIsNegative()
    {
        var repository = new Mock<IProcessingFileRepository>(MockBehavior.Strict);
        var analyzer = new Mock<IAnalyzerFileService>(MockBehavior.Strict);
        var mediator = new Mock<IMediator>(MockBehavior.Strict);
        var sut = new RegisterUploadedFileHandler(repository.Object, analyzer.Object, mediator.Object);

        var command = CreateCommand(size: -1);

        var response = await sut.Handle(command, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Reasons.Select(reason => reason.Message), Contains.Item("Size must be greater than or equal to zero"));
    }

    [Test]
    public async Task Handle_ShouldPersistPublishAndSendToAnalyzer_WhenInputIsValid()
    {
        var repository = new Mock<IProcessingFileRepository>();
        var analyzer = new Mock<IAnalyzerFileService>();
        var mediator = new Mock<IMediator>();

        repository
            .Setup(mock => mock.AddAsync(It.IsAny<ProcessingFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProcessingFile entity, CancellationToken _) => entity);

        mediator
            .Setup(mock => mock.Publish(It.IsAny<CreateEventLogNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        analyzer
            .Setup(mock => mock.SendAsync(It.IsAny<ProcessingFile>()))
            .Returns(Task.CompletedTask);

        var sut = new RegisterUploadedFileHandler(repository.Object, analyzer.Object, mediator.Object);
        var command = CreateCommand();

        var response = await sut.Handle(command, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data.FileId, Is.EqualTo(command.FileId));
        Assert.That(response.Data.Status, Is.EqualTo("RECEIVED"));

        repository.Verify(mock => mock.AddAsync(It.IsAny<ProcessingFile>(), It.IsAny<CancellationToken>()), Times.Once);
        mediator.Verify(mock => mock.Publish(It.IsAny<CreateEventLogNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        analyzer.Verify(mock => mock.SendAsync(It.IsAny<ProcessingFile>()), Times.Once);
    }

    private static RegisterUploadedFileCommand CreateCommand(
        string fileId = "file-1",
        string fileName = "video.mp4",
        string contentType = "video/mp4",
        long size = 100,
        string bucketName = "bucket",
        string key = "uploads/video.mp4")
    {
        return new RegisterUploadedFileCommand(
            fileId,
            fileName,
            contentType,
            size,
            DateTimeOffset.UtcNow,
            bucketName,
            key);
    }
}
