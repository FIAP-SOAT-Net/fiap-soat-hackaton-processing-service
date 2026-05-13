using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Tests.Entities;

public class ProcessingFileTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties_AndDefaultStatus()
    {
        var uploadedAt = DateTime.UtcNow;

        var sut = new ProcessingFile(
            "file-1",
            "document.pdf",
            "bucket-a",
            "uploads/document.pdf",
            1234,
            uploadedAt,
            "application/pdf");

        Assert.That(sut.FileId, Is.EqualTo("file-1"));
        Assert.That(sut.Name, Is.EqualTo("document.pdf"));
        Assert.That(sut.BucketName, Is.EqualTo("bucket-a"));
        Assert.That(sut.Key, Is.EqualTo("uploads/document.pdf"));
        Assert.That(sut.FileSizeBytes, Is.EqualTo(1234));
        Assert.That(sut.UploadedAt, Is.EqualTo(uploadedAt));
        Assert.That(sut.MimeType, Is.EqualTo("application/pdf"));
        Assert.That(sut.Status, Is.EqualTo("RECEIVED"));
    }

    [Test]
    public void UpdateStatus_ShouldChangeStatus_AndUpdateTimestamp()
    {
        var sut = new ProcessingFile(
            "file-1",
            "document.pdf",
            "bucket-a",
            "uploads/document.pdf",
            1234,
            DateTime.UtcNow.AddMinutes(-5),
            null);

        var beforeUpdate = sut.UpdatedAt;
        var result = sut.UpdateStatus("PROCESSING");

        Assert.That(result, Is.SameAs(sut));
        Assert.That(sut.Status, Is.EqualTo("PROCESSING"));
        Assert.That(sut.UpdatedAt, Is.GreaterThanOrEqualTo(beforeUpdate));
    }
}

