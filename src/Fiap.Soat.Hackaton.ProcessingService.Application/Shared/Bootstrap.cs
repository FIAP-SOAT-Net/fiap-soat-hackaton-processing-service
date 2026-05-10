using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Shared;

[ExcludeFromCodeCoverage]
public static class Bootstrap
{
    public static IServiceCollection AddInterfaceAdapters(this IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }
}
