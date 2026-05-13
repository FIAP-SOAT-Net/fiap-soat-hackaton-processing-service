using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.List;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Moq;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Tests.UseCases.File.List;

public class ListProcessingFilesHandlerTests
{
    [Test]
    public async Task Handle_ShouldReturnListFromRepository()
    {
        IReadOnlyList<ProcessingFile> files =
        [
            new ProcessingFile("file-1", "name-1", "bucket", "key-1", 10, DateTime.UtcNow, "text/plain"),
            new ProcessingFile("file-2", "name-2", "bucket", "key-2", 20, DateTime.UtcNow, "text/plain")
        ];

        var repository = new Mock<IProcessingFileRepository>();
        repository
            .Setup(mock => mock.GetAllAsync("f", "RECEIVED", null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        var sut = new ListProcessingFilesHandler(repository.Object);
        var query = new ListProcessingFilesQuery("f", "RECEIVED", null, null);

        var response = await sut.Handle(query, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data.Count, Is.EqualTo(2));
    }
}
