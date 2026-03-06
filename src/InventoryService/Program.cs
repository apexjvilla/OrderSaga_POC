using InventoryService.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // Consumers
    x.AddConsumer<ReserveInventoryConsumer>();
    x.AddConsumer<ReleaseInventoryConsumer>();

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

app.UseHttpsRedirection();

app.Run();
