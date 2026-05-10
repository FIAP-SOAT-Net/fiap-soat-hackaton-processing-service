using Microsoft.OpenApi;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            string[] xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string xmlFile in xmlFiles)
            {
                options.IncludeXmlComments(xmlFile, true);
            }
        });
        return services;
    }
}
