namespace Contracts.Commands
{
    public record ProcessPayment(
        Guid CorrelationId,
        Guid OrderId,
        decimal Amount,
        string CustomerEmail
        );
}
