using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Controllers;
using Fiap.Soat.Hackaton.ProcessingService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.Endpoints;

public static class ProcessingFileEndpoints
{
    public static WebApplication Map(WebApplication app)
    {
        app.MapPatch("/processingFiles/{fileId}",
                ([FromServices] IProcessingFilesController controller, [FromBody] UpdateFileProcessingRequest request, string fileId, CancellationToken cancellationToken) =>
                    controller.UpdateAsync(Guid.Parse(fileId), request, cancellationToken))
            .WithName("UpdateProcessingFileStatus")
            .WithDescription("Update a processing file status");

        app.MapGet("/processingFiles/{fileId:guid}",
                ([FromServices] IProcessingFilesController controller, Guid fileId, CancellationToken cancellationToken) =>
                    controller.GetAsync(fileId, cancellationToken))
            .WithName("GetProcessingFile")
            .WithDescription("Get a processing file by ID");

        app.MapGet("/processingFiles", ([FromServices] IProcessingFilesController controller, [AsParameters] GetProcessingFileQuery request,
                    CancellationToken cancellationToken) =>
                controller.GetAllAsync(request, cancellationToken))
            .WithName("ListProcessingFiles")
            .WithDescription("List all processing files");

        return app;
    }
}
