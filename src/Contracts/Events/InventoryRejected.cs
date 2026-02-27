namespace Contracts.Events
{
    public record InventoryRejected(
        Guid CorrelationId,
        Guid OrderId,
        string Reason
        );
}
