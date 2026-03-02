using MassTransit;

namespace OrderService.Domain.Sagas
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
        public bool InventoryReserved { get; set; }
        public bool PaymentProcessed { get; set; }
        public bool ShippingCreated { get; set; }
        public string CurrentState { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
