using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Sagas;

namespace OrderService.Persistence.Configurations
{
    public class OrderSagaStateConfiguration : IEntityTypeConfiguration<OrderSagaState>
    {
        public void Configure(EntityTypeBuilder<OrderSagaState> builder)
        {
            builder.HasKey(x => x.CorrelationId);

            builder.Property(x => x.CurrentState)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
