namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services.Messaging;

public class RabbitMqSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "fileservice.events.exchange";
    public string QueueName { get; set; } = "fileservice.events";
    public string RoutingKey { get; set; } = "file.uploaded.*";
}

