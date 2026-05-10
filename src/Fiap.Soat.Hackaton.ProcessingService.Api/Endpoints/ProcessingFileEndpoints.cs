using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.Endpoints;

public static class ProcessingFileEndpoints
{
    public static WebApplication Map(WebApplication app)
    {
        app.MapPatch("/processingFiles/{fileId}", ([FromBody] UpdateFileProcessingRequest request, string fileId, CancellationToken cancellationToken) => Results.Ok(new { request, fileId }))
            .WithName("UpdateProcessingFileStatus")
            .WithDescription("Update a processing file status");

        app.MapGet("/processingFiles/{fileId}", (string fileId, CancellationToken cancellationToken) => Results.Ok(new GetFileByIdQuery(fileId)))
            .WithName("GetProcessingFile")
            .WithDescription("Get a processing file by ID");

        app.MapGet("/processingFiles", ([AsParameters] GetProcessingFileQuery request, CancellationToken cancellationToken) => Results.Ok(request))
            .WithName("ListProcessingFiles")
            .WithDescription("List all processing files");

        return app;
    }
}

public record UpdateFileProcessingRequest(string Status);
public record GetFileByIdQuery(string FileId);
public record GetProcessingFileQuery(string? FileId, string? Status, DateTime? StartDate, DateTime? EndDate);
