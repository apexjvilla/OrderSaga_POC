namespace OrderService.Domain.Sagas
{
    public class OrderSagaState
    {
        public Guid CorrelationId { get; set; }

        public Guid OrderId { get; set; }

        public bool InventoryReserved { get; set; }

        public bool PaymentProcessed { get; set; }

        public bool ShippingCreated { get; set; }

        public OrderSagaStatus CurrentState { get; set; } = OrderSagaStatus.Started;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
