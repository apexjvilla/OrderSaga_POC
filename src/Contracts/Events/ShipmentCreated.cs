namespace Contracts.Events
{
    public record ShipmentCreated(
        Guid CorrelationId,
        Guid OrderId,
        string TrackingNumber
        );
}
