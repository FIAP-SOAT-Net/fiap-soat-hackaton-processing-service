using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.Shared;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.Repositories;

public class ProcessingFileRepositoryTests
{
    [Test]
    public async Task AddAsync_AndGetByIdAsync_ShouldPersistAndRetrieveEntity()
    {
        await using var scope = InfrastructureTestContext.CreateSqliteContext();
        var repository = new ProcessingFileRepository(scope.Context);

        var entity = new ProcessingFile("file-1", "video.mp4", "bucket-a", "uploads/video.mp4", 1024, DateTime.UtcNow, "video/mp4");

        var inserted = await repository.AddAsync(entity, CancellationToken.None);
        var reloaded = await repository.GetByIdAsync(inserted.Id, CancellationToken.None);

        Assert.That(inserted, Is.SameAs(entity));
        Assert.That(inserted.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(reloaded, Is.Not.Null);
        Assert.That(reloaded!.FileId, Is.EqualTo("file-1"));
        Assert.That(reloaded.Name, Is.EqualTo("video.mp4"));
    }

    [Test]
    public async Task GetAllAsync_ShouldFilterByFileIdStatusAndDateRange()
    {
        await using var scope = InfrastructureTestContext.CreateSqliteContext();
        var repository = new ProcessingFileRepository(scope.Context);

        var start = new DateTime(2026, 01, 01, 10, 0, 0, DateTimeKind.Utc);
        var middle = new DateTime(2026, 01, 02, 10, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 01, 03, 10, 0, 0, DateTimeKind.Utc);

        await repository.AddAsync(CreateFile("match-1", "RECEIVED", start), CancellationToken.None);
        await repository.AddAsync(CreateFile("match-2", "PROCESSING", middle), CancellationToken.None);
        await repository.AddAsync(CreateFile("other", "RECEIVED", end), CancellationToken.None);

        var results = await repository.GetAllAsync("match-2", "PROCESSING", middle.AddMinutes(-1), middle.AddMinutes(1), CancellationToken.None);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results.Single().FileId, Is.EqualTo("match-2"));
        Assert.That(results.Single().Status, Is.EqualTo("PROCESSING"));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllWhenNoFiltersAreProvided()
    {
        await using var scope = InfrastructureTestContext.CreateSqliteContext();
        var repository = new ProcessingFileRepository(scope.Context);

        await repository.AddAsync(CreateFile("file-1", "RECEIVED", DateTime.UtcNow.AddMinutes(-2)), CancellationToken.None);
        await repository.AddAsync(CreateFile("file-2", "PROCESSING", DateTime.UtcNow.AddMinutes(-1)), CancellationToken.None);

        var results = await repository.GetAllAsync(null, null, null, null, CancellationToken.None);

        Assert.That(results, Has.Count.EqualTo(2));
    }

    private static ProcessingFile CreateFile(string fileId, string status, DateTime createdAt)
    {
        var entity = new ProcessingFile(fileId, fileId + ".mp4", "bucket", fileId + ".mp4", 100, createdAt, "video/mp4");
        entity.UpdateStatus(status);
        SetCreatedAt(entity, createdAt);
        return entity;
    }

    private static void SetCreatedAt(ProcessingFile entity, DateTime value)
    {
        var property = typeof(Entity).GetProperty(nameof(Entity.CreatedAt));
        property!.SetValue(entity, value);
    }
}

