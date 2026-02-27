namespace Contracts.Events
{
    public record PaymentSucceded(
        Guid CorrelationId,
        Guid OrderId
        );
}
