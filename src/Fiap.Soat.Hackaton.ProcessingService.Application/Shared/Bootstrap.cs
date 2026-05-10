using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Shared;

[ExcludeFromCodeCoverage]
public static class Bootstrap
{
    public static IServiceCollection AddInterfaceAdapters(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<IProcessingFilesController, ProcessingFilesController>();
        return serviceCollection;
    }
}
