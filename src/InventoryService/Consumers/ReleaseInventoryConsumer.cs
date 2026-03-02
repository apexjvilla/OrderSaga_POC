using Contracts.Commands;
using MassTransit;

namespace InventoryService.Consumers
{
    public class ReleaseInventoryConsumer(ILogger<ReleaseInventoryConsumer> logger) : IConsumer<ReleaseInventory>
    {
        private readonly ILogger<ReleaseInventoryConsumer> _logger = logger;

        public  async Task Consume(ConsumeContext<ReleaseInventory> context)
        {
            var msg = context.Message;

            _logger.LogInformation(
                $"ReleaseInventory received CorrelationId={msg.CorrelationId} OrderId={msg.OrderId} ProductId={msg.ProductId} Qty={msg.Quantity}");

            await Task.Delay(2000);


        }
    }
}
