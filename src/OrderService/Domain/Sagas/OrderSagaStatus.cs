namespace OrderService.Domain.Sagas
{
    public enum OrderSagaStatus
    {
        Started = 0,
        InventoryReserved = 1,
        PaymentProcessed = 2,
        ShippingCreated = 3,
        Completed = 4,
        Failed = 5,
        Cancelled = 6
    }
}
