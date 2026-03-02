namespace Contracts.Events
{
    public record ShippmentFailed(
        Guid CorrelationId,
        Guid OrderId,
        string Reason
        );
}
