using MassTransit;
using Microsoft.EntityFrameworkCore;
using Product.Core.Events;
using Product.Infrastructure.Data;
using System.Text.Json;

namespace Product.Api;

public class Hosted(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastUpdate = DateTime.MinValue;

        using var scope = serviceProvider.CreateAsyncScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        var provider = scope.ServiceProvider.GetRequiredService<ITopicProducerProvider>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = await dbContext.Outbox
                .Where(c => c.CreatedAt >= lastUpdate)
                .ToArrayAsync(stoppingToken);

            if (messages is { Length: > 0 })
            {
                var producer = provider.GetProducer<ProductCreatedEvent>(new Uri("topic:test-topic"));
                foreach (var message in messages)
                {
                    var data = JsonSerializer.Deserialize<ProductCreatedEvent>(message.Payload);

                    await producer.Produce(data, stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
