using Fiap.Soat.Hackaton.ProcessingService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Controllers;

public interface IProcessingFilesController
{
    Task<IActionResult> UpdateAsync(Guid id, UpdateFileProcessingRequest request, CancellationToken cancellationToken);
    Task<ActionResult> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<ActionResult> GetAllAsync(GetProcessingFileQuery request, CancellationToken cancellationToken);
}
