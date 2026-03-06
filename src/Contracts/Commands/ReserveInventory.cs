namespace Contracts.Commands
{
    public record ReserveInventory(
        Guid CorrelationId,
        Guid OrderId,
        Guid ProductId,
        int Quantity
        );
}
