using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OrderService.Domain.Sagas;
using OrderService.Domain.Sagas.Activities;
using OrderService.Infrastructure.Mongo;
using OrderService.Persistence;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(
    new GuidSerializer(GuidRepresentation.Standard)
);

// Add dbContext
builder.Services.AddDbContext<SagaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")));

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddSagaStateMachine<OrderStateMachine, OrderSagaState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;

            r.AddDbContext<DbContext, SagaDbContext>((provider, cfg) =>
            {
                cfg.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            builder.Configuration["RabbitMq:Host"],
            builder.Configuration["RabbitMq:VirtualHost"],
            h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

        cfg.ConfigureEndpoints(context);
    });
});

// Add Mongo db
builder.Services.AddSingleton<MongoSagaLogger>();

builder.Services.AddScoped(typeof(LogOrderSagaActivity<>));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapControllers();

// Auto apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SagaDbContext>();

    int retries = 10;

    while (retries > 0)
    {
        try
        {
            Console.WriteLine("Applying database migrations...");
            db.Database.Migrate();
            Console.WriteLine("Database migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            retries--;

            Console.WriteLine($"Database not ready yet. Retrying in 3 seconds... Attempts left: {retries}");
            Console.WriteLine(ex.Message);

            Thread.Sleep(3000);
        }
    }
}

// Endpoints
#region Endpoints
app.MapGet("/", () => "OrderService running");

app.MapPost("/orders", async (IPublishEndpoint publishEndpoint) =>
{
    var correlationId = Guid.NewGuid();
    var orderId = Guid.NewGuid();

    await publishEndpoint.Publish(new OrderSubmitted(
        correlationId,
        orderId,
        "Product 1",
        25));

    return Results.Accepted($"/orders/{orderId}", new
    {
        Message = "Order creation started",
        OrderId = orderId
    });
});
#endregion

app.Run();
