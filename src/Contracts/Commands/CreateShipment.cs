namespace Contracts.Commands
{
    public record CreateShipment(
        Guid CorrelationId,
        Guid OrderId,
        string Address
        );
}
