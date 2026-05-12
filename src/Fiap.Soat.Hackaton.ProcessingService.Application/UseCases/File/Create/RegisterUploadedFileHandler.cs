using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.EventLog;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using MediatR;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Create;

public sealed class RegisterUploadedFileHandler(IProcessingFileRepository repository, IMediator mediator)
    : IRequestHandler<RegisterUploadedFileCommand, Response<ProcessingFile>>
{
    public async Task<Response<ProcessingFile>> Handle(RegisterUploadedFileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.FileId))
        {
            return ResponseFactory.Fail<ProcessingFile>("FileId is required");
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return ResponseFactory.Fail<ProcessingFile>("FileName is required");
        }

        if (string.IsNullOrWhiteSpace(request.BucketName))
        {
            return ResponseFactory.Fail<ProcessingFile>("BucketName is required");
        }

        if (string.IsNullOrWhiteSpace(request.Key))
        {
            return ResponseFactory.Fail<ProcessingFile>("Key is required");
        }

        if (request.Size < 0)
        {
            return ResponseFactory.Fail<ProcessingFile>("Size must be greater than or equal to zero");
        }

        var entity = new ProcessingFile(
            request.FileId,
            request.FileName,
            request.BucketName,
            request.Key,
            request.Size,
            request.Timestamp.UtcDateTime,
            request.ContentType);
        _ = await repository.AddAsync(entity, cancellationToken);
        await mediator.Publish(new CreateEventLogNotification(entity.Id, null, "RECEIVED"), cancellationToken);

        return ResponseFactory.Ok(entity);
    }
}


