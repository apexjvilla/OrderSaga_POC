namespace OrderService.Infrastructure.Mongo
{
    public class OrderSagaLog
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
        public string Event { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Payload { get; set; } = string.Empty;
    }
}
