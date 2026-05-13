using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Get;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Moq;
using System.Net;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Tests.UseCases.File.Get;

public class GetProcessingFileByIdHandlerTests
{
    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenEntityDoesNotExist()
    {
        var repository = new Mock<IProcessingFileRepository>();
        repository
            .Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProcessingFile?) null);

        var sut = new GetProcessingFileByIdHandler(repository.Object);
        var query = new GetProcessingFileByIdQuery(Guid.NewGuid());

        var response = await sut.Handle(query, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(response.Reasons.Select(reason => reason.Message), Contains.Item("Processing file Not Found"));
    }

    [Test]
    public async Task Handle_ShouldReturnOk_WhenEntityExists()
    {
        var entity = new ProcessingFile("file-1", "name", "bucket", "key", 10, DateTime.UtcNow, "text/plain");
        var repository = new Mock<IProcessingFileRepository>();
        repository
            .Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var sut = new GetProcessingFileByIdHandler(repository.Object);
        var query = new GetProcessingFileByIdQuery(Guid.NewGuid());

        var response = await sut.Handle(query, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.SameAs(entity));
    }
}
