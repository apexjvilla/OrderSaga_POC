using Contracts.Commands;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace InventoryService.Consumers
{
    public class ReserveInventoryConsumer(ILogger<ReserveInventoryConsumer> logger) : IConsumer<ReserveInventory>
    {
        private readonly ILogger<ReserveInventoryConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<ReserveInventory> context)
        {
            var msg = context.Message;

            _logger.LogInformation(
                $"ReserveInventory received CorrelationId={msg.CorrelationId} OrderId={msg.OrderId} ProductId={msg.ProductId} Qty={msg.Quantity}");

            var canReserve = msg.Quantity > 0;

            if (!canReserve)
            {
                await context.Publish(new InventoryRejected(
                    msg.CorrelationId,
                    msg.OrderId,
                    "Invalid quantity"));

                return;
            }

            await Task.Delay(2000);

            await context.Publish(new InventoryReserved(
                msg.CorrelationId,
                msg.OrderId));
            ;        }
    }
}
