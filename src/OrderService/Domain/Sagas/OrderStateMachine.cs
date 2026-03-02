using Contracts.Commands;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrderService.Domain.Sagas
{
    public class OrderStateMachine : MassTransitStateMachine<OrderSagaState>
    {
        private readonly ILogger<OrderStateMachine> _logger;

        public State WaitingForInventory { get; private set; } = null!;
        public State WaitingForPayment { get; private set; } = null!;
        public State WaitingForShipping { get; private set; } = null!;
        public State Completed { get; private set; } = null!;
        public State Failed { get; private set; } = null!;

        public Event<OrderSubmitted> OrderSubmitted { get; private set; } = null!;
        public Event<InventoryReserved> InventoryReserved { get; private set; } = null!;
        public Event<InventoryRejected> InventoryRejected { get; private set; } = null!;
        public Event<PaymentSucceeded> PaymentSucceeded { get; private set; } = null!;
        public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;
        public Event<ShipmentCreated> ShippmentCreated { get; private set; } = null!;
        public Event<ShipmentFailed> ShippmentFailed { get; private set; } = null!;

        public OrderStateMachine(ILogger<OrderStateMachine> logger)
        {
            _logger = logger;

            InstanceState(x => x.CurrentState);

            Event(() => OrderSubmitted, x =>
            {
                x.CorrelateById(m => m.Message.CorrelationId);
                x.InsertOnInitial = true;

                x.SetSagaFactory(ctx => new OrderSagaState
                {
                    CorrelationId = ctx.Message.CorrelationId,
                    OrderId = ctx.Message.OrderId,
                    CreatedAt = DateTime.UtcNow
                });
            });

            Event(() => InventoryReserved,
                x => x.CorrelateById(m => m.Message.CorrelationId));

            Event(() => InventoryRejected,
                x => x.CorrelateById(m => m.Message.CorrelationId));

            Event(() => PaymentSucceeded,
                x => x.CorrelateById(m => m.Message.CorrelationId));

            Event(() => PaymentFailed,
                x => x.CorrelateById(m => m.Message.CorrelationId));

            Event(() => ShippmentCreated,
                x => x.CorrelateById(m => m.Message.CorrelationId));

            Event(() => ShippmentFailed,
                x => x.CorrelateById(m => m.Message.CorrelationId));

            Initially(
                When(OrderSubmitted)
                    .Then(ctx =>
                    {
                        ctx.Saga.OrderId = ctx.Message.OrderId;

                        _logger.LogInformation(
                            "Saga started CorrelationId={CorrelationId} OrderId={OrderId}",
                            ctx.Saga.CorrelationId,
                            ctx.Saga.OrderId);
                    })
                    .Send(new Uri("queue:reserve-inventory"), ctx =>
                        new ReserveInventory(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.OrderId,
                            ProductId: Guid.NewGuid(),
                            Quantity: ctx.Message.Quantity))
                    .TransitionTo(WaitingForInventory)
            );

            During(WaitingForInventory,
                When(InventoryReserved)
                    .Then(ctx =>
                    {
                        ctx.Saga.InventoryReserved = true;

                        _logger.LogInformation(
                            "Inventory reserved CorrelationId={CorrelationId}",
                            ctx.Saga.CorrelationId);
                    })
                    .Send(new Uri("queue:process-payment"), ctx =>
                        new ProcessPayment(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.OrderId,
                            Amount: 100m,
                            CustomerEmail: "customer@test.com"))
                    .TransitionTo(WaitingForPayment),

                When(InventoryRejected)
                    .Then(ctx =>
                    {
                        _logger.LogWarning(
                            "Inventory rejected CorrelationId={CorrelationId} Reason={Reason}",
                            ctx.Saga.CorrelationId,
                            ctx.Message.Reason);
                    })
                    .TransitionTo(Failed)
                    .Finalize()
            );

            During(WaitingForPayment,
                When(PaymentSucceeded)
                    .Then(ctx =>
                    {
                        ctx.Saga.PaymentProcessed = true;

                        _logger.LogInformation(
                            "Payment succeeded CorrelationId={CorrelationId}",
                            ctx.Saga.CorrelationId);
                    })
                    .Send(new Uri("queue:create-shipment"), ctx =>
                        new CreateShipment(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.OrderId,
                            Address: "Fake address 123"))
                    .TransitionTo(WaitingForShipping),

                When(PaymentFailed)
                    .Then(ctx =>
                    {
                        _logger.LogWarning(
                            "Payment failed CorrelationId={CorrelationId} Reason={Reason}",
                            ctx.Saga.CorrelationId,
                            ctx.Message.Reason);
                    })
                    .Send(new Uri("queue:release-inventory"), ctx =>
                        new ReleaseInventory(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.OrderId,
                            ProductId: Guid.NewGuid(),
                            Quantity: 1))
                    .TransitionTo(Failed)
                    .Finalize()
            );

            During(WaitingForShipping,
                When(ShippmentCreated)
                    .Then(ctx =>
                    {
                        ctx.Saga.ShippingCreated = true;

                        _logger.LogInformation(
                            "Shipment created CorrelationId={CorrelationId}",
                            ctx.Saga.CorrelationId);
                    })
                    .TransitionTo(Completed)
                    .Finalize(),

                When(ShippmentFailed)
                    .Then(ctx =>
                    {
                        _logger.LogWarning(
                            "Shipment failed CorrelationId={CorrelationId} Reason={Reason}",
                            ctx.Saga.CorrelationId,
                            ctx.Message.Reason);
                    })
                    .Send(new Uri("queue:refund-payment"), ctx =>
                        new RefundPayment(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.OrderId,
                            Amount: 100m))
                    .Send(new Uri("queue:release-inventory"), ctx =>
                        new ReleaseInventory(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.OrderId,
                            ProductId: Guid.NewGuid(),
                            Quantity: 1))
                    .TransitionTo(Failed)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}
