using MassTransit;
using OrderService.Infrastructure.Mongo;

namespace OrderService.Domain.Sagas.Activities
{
    public class LogOrderSagaActivity<TMessage> :
    IStateMachineActivity<OrderSagaState, TMessage>
    where TMessage : class
    {
        private readonly MongoSagaLogger _logger;

        public LogOrderSagaActivity(MongoSagaLogger logger)
        {
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("log-saga");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(
            BehaviorContext<OrderSagaState, TMessage> context,
            IBehavior<OrderSagaState, TMessage> next)
        {
            await _logger.Log(
                context.Saga.CorrelationId,
                context.Saga.OrderId,
                typeof(TMessage).Name,
                context.Saga.CurrentState ?? "Unknown",
                context.Message);

            await next.Execute(context);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderSagaState, TMessage, TException> context,
            IBehavior<OrderSagaState, TMessage> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}