using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Create;

public sealed record RegisterUploadedFileCommand : IRequest<Response<ProcessingFile>>
{
    public string FileId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public string BucketName { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;

    public RegisterUploadedFileCommand(string fileId, string fileName, string contentType, long size, DateTimeOffset timestamp, string bucketName, string key)
    {
        FileId = fileId;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        Timestamp = timestamp;
        BucketName = bucketName;
        Key = key;
    }
}


