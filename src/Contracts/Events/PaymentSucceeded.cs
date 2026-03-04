namespace Contracts.Events
{
    public record PaymentSucceeded(
        Guid CorrelationId,
        Guid OrderId
        );
}
