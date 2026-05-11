using Fiap.Soat.Hackaton.ProcessingService.Application.Models;
using Fiap.Soat.Hackaton.ProcessingService.Application.UseCases.File.Create;
using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Services.Messaging;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Fiap.Soat.Hackaton.ProcessingService.Api.BackgroundServices;

public sealed class MessagesConsumer(
    ILogger<MessagesConsumer> logger,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<RabbitMqSettings> options) : BackgroundService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var connection = CreateConnection();
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(
                    exchange: options.Value.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                channel.QueueDeclare(
                    queue: options.Value.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(
                    queue: options.Value.QueueName,
                    exchange: options.Value.ExchangeName,
                    routingKey: options.Value.RoutingKey);

                channel.BasicQos(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += (_, eventArgs) => HandleMessageAsync(channel, eventArgs, stoppingToken);

                var consumerCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                connection.ConnectionShutdown += (_, _) => consumerCompletion.TrySetResult();
                channel.ModelShutdown += (_, _) => consumerCompletion.TrySetResult();

                _ = channel.BasicConsume(
                    queue: options.Value.QueueName,
                    autoAck: false,
                    consumer: consumer);

                logger.LogInformation(
                    "Listening on RabbitMQ queue {Queue} from exchange {Exchange} with routing key {RoutingKey}",
                    options.Value.QueueName,
                    options.Value.ExchangeName,
                    options.Value.RoutingKey);

                await consumerCompletion.Task.WaitAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "RabbitMQ consumer stopped unexpectedly. Retrying in 5 seconds.");

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Value.Host,
            Port = options.Value.Port,
            UserName = options.Value.User,
            Password = options.Value.Password,
            DispatchConsumersAsync = true
        };

        return factory.CreateConnection();
    }

    private async Task HandleMessageAsync(IModel channel, BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonSerializer.Deserialize<FileUploadedEvent>(eventArgs.Body.Span, _jsonSerializerOptions);
            if (message is null) throw new JsonException("RabbitMQ message body could not be deserialized.");

            using var scope = serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var response = await mediator.Send(
                new RegisterUploadedFileCommand(
                    message.FileId,
                    message.FileName,
                    message.ContentType,
                    message.Size,
                    message.Timestamp,
                    message.BucketName,
                    message.Key),
                cancellationToken);

            if (!response.IsSuccess)
            {
                logger.LogWarning(
                    "RabbitMQ message {DeliveryTag} was processed with validation errors: {Reasons}",
                    eventArgs.DeliveryTag,
                    string.Join(" | ", response.Reasons.Select(reason => reason.Message)));
            }

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process RabbitMQ message {DeliveryTag}", eventArgs.DeliveryTag);
            channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
        }
    }
}
