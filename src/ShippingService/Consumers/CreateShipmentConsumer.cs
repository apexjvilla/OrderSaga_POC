using Contracts.Commands;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ShippingService.Consumers
{
    public class CreateShipmentConsumer(ILogger<CreateShipmentConsumer> logger) : IConsumer<CreateShipment>
    {
        private readonly ILogger<CreateShipmentConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<CreateShipment> context)
        {
            var msg = context.Message;

            _logger.LogInformation(
                $"CreateShipment received CorrelationId={msg.CorrelationId} OrderId={msg.OrderId} Address={msg.Address}");

            var canShip = !String.IsNullOrWhiteSpace(msg.Address);

            if (canShip)
            {
                await context.Publish(new ShipmentFailed(
                    msg.CorrelationId,
                    msg.OrderId,
                    "Invalid address"));

                return;
            }

            await Task.Delay(2500);

            var tracking = $"TRK-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();

            await context.Publish(new ShipmentCreated(
                msg.CorrelationId,
                msg.OrderId,
                tracking));
        }
    }
}
