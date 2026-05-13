using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.Data;

public class AppDbContextModelTests
{
    [Test]
    public void ProcessingFileConfiguration_ShouldMapExpectedTableColumnsAndIndexes()
    {
        using var scope = InfrastructureTestContext.CreateSqliteContext();
        var context = scope.Context;

        var entityType = context.Model.FindEntityType(typeof(ProcessingFile));
        Assert.That(entityType, Is.Not.Null);
        Assert.That(entityType!.GetTableName(), Is.EqualTo("processing_files"));

        var table = StoreObjectIdentifier.Table("processing_files", null);

        Assert.Multiple(() =>
        {
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.Name))!.GetColumnName(table), Is.EqualTo("name"));
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.FileId))!.GetColumnName(table), Is.EqualTo("file_id"));
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.BucketName))!.GetColumnName(table), Is.EqualTo("bucket_name"));
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.Key))!.GetColumnName(table), Is.EqualTo("key"));
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.FileSizeBytes))!.GetColumnName(table), Is.EqualTo("file_size_bytes"));
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.MimeType))!.GetColumnName(table), Is.EqualTo("mime_type"));
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.UploadedAt))!.GetColumnName(table), Is.EqualTo("uploaded_at"));
            Assert.That(entityType.FindProperty(nameof(ProcessingFile.Status))!.GetColumnName(table), Is.EqualTo("status"));
            Assert.That(entityType.FindIndex(entityType.FindProperty(nameof(ProcessingFile.Status))!)!.GetDatabaseName(), Is.EqualTo("idx_files_status"));
            Assert.That(entityType.FindIndex(entityType.FindProperty(nameof(ProcessingFile.UploadedAt))!)!.GetDatabaseName(), Is.EqualTo("idx_files_uploaded_at"));
        });
    }

    [Test]
    public void ProcessingEventLogConfiguration_ShouldMapExpectedColumns_AndIgnoreUpdatedAt()
    {
        using var scope = InfrastructureTestContext.CreateSqliteContext();
        var context = scope.Context;

        var entityType = context.Model.FindEntityType(typeof(ProcessingEventLog));
        Assert.That(entityType, Is.Not.Null);
        Assert.That(entityType!.GetTableName(), Is.EqualTo("event_logs"));
        Assert.That(entityType.FindProperty(nameof(Entity.UpdatedAt)), Is.Null);

        var table = StoreObjectIdentifier.Table("event_logs", null);

        Assert.Multiple(() =>
        {
            Assert.That(entityType.FindProperty(nameof(ProcessingEventLog.ProcessingFileId))!.GetColumnName(table), Is.EqualTo("processing_file_id"));
            Assert.That(entityType.FindProperty(nameof(ProcessingEventLog.EventType))!.GetColumnName(table), Is.EqualTo("event_type"));
            Assert.That(entityType.FindProperty(nameof(ProcessingEventLog.StatusFrom))!.GetColumnName(table), Is.EqualTo("status_from"));
            Assert.That(entityType.FindProperty(nameof(ProcessingEventLog.StatusTo))!.GetColumnName(table), Is.EqualTo("status_to"));
            Assert.That(entityType.FindIndex(entityType.FindProperty(nameof(ProcessingEventLog.ProcessingFileId))!)!.GetDatabaseName(), Is.EqualTo("idx_event_logs_file_id"));
            Assert.That(entityType.FindIndex(entityType.FindProperty(nameof(ProcessingEventLog.EventType))!)!.GetDatabaseName(), Is.EqualTo("idx_event_logs_event_type"));
            Assert.That(entityType.FindIndex(entityType.FindProperty(nameof(Entity.CreatedAt))!)!.GetDatabaseName(), Is.EqualTo("idx_event_logs_created_at"));
        });
    }
}

