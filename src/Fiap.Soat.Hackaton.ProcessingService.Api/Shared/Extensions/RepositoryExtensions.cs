using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryExtensions(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<IProcessingEventLogRepository, ProcessingEventLogRepository>();
        _ = serviceCollection.AddScoped<IProcessingFileRepository, ProcessingFileRepository>();

        return serviceCollection;
    }
}
