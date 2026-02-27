namespace Contracts.Events
{
    public record InventoryReserved(
        Guid CorrelationId,
        Guid OrderId
        );
}
