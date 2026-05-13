using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Services;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<IAnalyzerFileService, AnalyzerFileService>();
        return serviceCollection;
    }
}
