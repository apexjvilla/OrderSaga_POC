using Contracts.Commands;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentService.Consumers
{
    public class ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger) : IConsumer<ProcessPayment>
    {
        private readonly ILogger<ProcessPaymentConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<ProcessPayment> context)
        {
            var msg = context.Message;

            _logger.LogInformation(
                $"ProcessPayment received CorrelationId={msg.CorrelationId} OrderId={msg.OrderId} Amount={msg.Amount} Email={msg.CustomerEmail}");

            var paymentOk = msg.Amount > 0;

            if (!paymentOk)
            {
                await context.Publish(new PaymentFailed(
                    msg.CorrelationId,
                    msg.OrderId,
                    "Invalid amount"));

                return;
            }

            await Task.Delay(1000);

            await context.Publish(new PaymentSucceeded(
                msg.CorrelationId,
                msg.OrderId));
        }
    }
}
