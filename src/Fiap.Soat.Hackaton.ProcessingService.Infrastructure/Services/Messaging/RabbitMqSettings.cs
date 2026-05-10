namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services.Messaging;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "database.events.exchange";
    public string QueueName { get; set; } = "database.events";
}

