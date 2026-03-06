namespace Contracts.Commands
{
    public record ReleaseInventory(
        Guid CorrelationId,
        Guid OrderId,
        Guid ProductId,
        int Quantity
        );
}
