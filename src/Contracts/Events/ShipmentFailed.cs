namespace Contracts.Events
{
    public record ShipmentFailed(
        Guid CorrelationId,
        Guid OrderId,
        string Reason
        );
}
