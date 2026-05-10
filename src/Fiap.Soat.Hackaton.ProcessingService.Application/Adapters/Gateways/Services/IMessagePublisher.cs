namespace Fiap.Soat.Hackaton.ProcessingService.Application.Adapters.Gateways.Services;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync<T>(string exchangeName, string routingKey, T message, CancellationToken cancellationToken = default) where T : class;
}
