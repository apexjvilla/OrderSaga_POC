using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add dbContext
builder.Services.AddDbContext<SagaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")));

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

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

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Auto apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SagaDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "OrderService running");

app.UseHttpsRedirection();

app.Run();
