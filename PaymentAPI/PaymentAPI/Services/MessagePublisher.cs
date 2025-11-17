using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using PaymentAPI.Services.Interfaces;

namespace PaymentAPI.Services;

/// <summary>
/// Service for publishing events to message queue
/// </summary>
public class MessagePublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<MessagePublisher> _logger;

    public MessagePublisher(ILogger<MessagePublisher> logger)
    {
        _logger = logger;

        // Create connection to RabbitMQ
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare exchange
        _channel.ExchangeDeclare("salessystem.events", ExchangeType.Topic, durable: true);
    }

    /// <summary>
    /// Publish payment validation event
    /// </summary>
    public void PublishPaymentValidated(PaymentValidatedEvent @event)
    {
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "salessystem.events",
            routingKey: "payment.validated",
            basicProperties: null,
            body: body);

        _logger.LogInformation("Published payment validated event for card {CardId}", @event.CardId);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

/// <summary>
/// Event data for payment validation
/// </summary>
public class PaymentValidatedEvent
{
    public string CardId { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty; // Masked
    public bool IsValid { get; set; }
    public DateTime ValidatedAt { get; set; }
    public string UserId { get; set; } = string.Empty; // If available
}