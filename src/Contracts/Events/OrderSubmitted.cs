namespace Contracts.Events
{
    public record OrderSubmitted(
        Guid CorrelationId,
        Guid OrderId,
        string ProductName,
        int Quantity
        );
}
