using Fiap.Soat.Hackaton.ProcessingService.Domain.DTOs;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Presenters;

public static class ProcessingFilePresenter
{
    public static ProcessingFileDto ToDto(ProcessingFile entity) =>
        new ProcessingFileDto(entity.Id, entity.Name, entity.FileId, entity.BucketName, entity.Key, entity.FileSizeBytes, entity.MimeType, entity.UploadedAt, entity.Status);

    public static IReadOnlyList<ProcessingFileDto> ToDto(IReadOnlyList<ProcessingFile> entities) =>
        entities.Select(ToDto).ToList();
}
