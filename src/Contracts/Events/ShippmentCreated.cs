namespace Contracts.Events
{
    public record ShippmentCreated(
        Guid CorrelationId,
        Guid OrderId,
        string TrackingNumber
        );
}
