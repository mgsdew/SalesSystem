namespace PaymentAPI.Services.Interfaces;

/// <summary>
/// Interface for message publishing
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publish payment validation event
    /// </summary>
    void PublishPaymentValidated(PaymentValidatedEvent @event);
}