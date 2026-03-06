using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Sagas;

namespace OrderService.Persistence
{
    public class SagaDbContext : DbContext
    {
        public SagaDbContext(DbContextOptions<SagaDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderSagaState> OrderSagas => Set<OrderSagaState>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SagaDbContext).Assembly);
        }
    }
}
