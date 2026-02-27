namespace Contracts.Events
{
    public record PaymentFailed(
        Guid CorrelationId,
        Guid OrderId,
        string Reason
        );
}
