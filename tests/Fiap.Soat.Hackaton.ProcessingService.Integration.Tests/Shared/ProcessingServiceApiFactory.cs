using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Services;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Entities;

namespace Fiap.Soat.Hackaton.ProcessingService.Integration.Tests.Shared;

public sealed class ProcessingServiceApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ProcessingServiceApiFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHostedService>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IAnalyzerFileService>();
            services.RemoveAll<IMessagePublisher>();

            services.AddSingleton(_connection);
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
            services.AddScoped<IAnalyzerFileService, NoOpAnalyzerFileService>();
            services.AddSingleton<IMessagePublisher, NoOpMessagePublisher>();
        });
    }

    public HttpClient CreateHttpsClient()
        => CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task<ProcessingFile> SeedProcessingFileAsync(ProcessingFile entity)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.ProcessingFiles.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }

    private sealed class NoOpAnalyzerFileService : IAnalyzerFileService
    {
        public Task SendAsync(ProcessingFile processingFile) => Task.CompletedTask;
    }

    private sealed class NoOpMessagePublisher : IMessagePublisher
    {
        public Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default) where T : class
            => Task.CompletedTask;

        public Task PublishAsync<T>(string exchangeName, string routingKey, T message, CancellationToken cancellationToken = default) where T : class
            => Task.CompletedTask;
    }
}


