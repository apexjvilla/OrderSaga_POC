using MongoDB.Driver;
using System.Text.Json;

namespace OrderService.Infrastructure.Mongo
{
    public class MongoSagaLogger
    {
        private readonly IMongoCollection<OrderSagaLog> _collection;

        public MongoSagaLogger()
        {
            var client = new MongoClient("mongodb://mongodb:27017");

            var database = client.GetDatabase("OrderSagaLogs");

            _collection = database.GetCollection<OrderSagaLog>("logs");
        }

        public async Task Log(
            Guid correlationId,
            Guid orderId,
            string eventName,
            string state,
            object payload)
        {
            var log = new OrderSagaLog
            {
                Id = Guid.NewGuid(),
                CorrelationId = correlationId,
                OrderId = orderId,
                Event = eventName,
                State = state,
                Timestamp = DateTime.UtcNow,
                Payload = JsonSerializer.Serialize(payload)
            };

            await _collection.InsertOneAsync(log);
        }
    }
}
