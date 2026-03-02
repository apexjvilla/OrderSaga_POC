namespace Contracts.Commands
{
    public record RefundPayment(
        Guid CorrelationId,
        Guid OrderId,
        decimal Amount
        );
}
