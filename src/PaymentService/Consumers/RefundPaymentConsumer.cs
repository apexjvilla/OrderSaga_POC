using Contracts.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentService.Consumers
{
    public class RefundPaymentConsumer(ILogger<RefundPaymentConsumer> logger) : IConsumer<RefundPayment>
    {
        private readonly ILogger<RefundPaymentConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<RefundPayment> context)
        {
            var msg = context.Message;

            _logger.LogInformation(
                $"RefundPayment received CorrelationId={msg.CorrelationId} OrderId={msg.OrderId} Amount={msg.Amount}");

            await Task.Delay(200);


        }
    }
}
