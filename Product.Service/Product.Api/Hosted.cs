using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Product.Infrastructure.Data;

namespace Product.Api;

public class Hosted(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastUpdate = DateTime.MinValue;

        using var scope = serviceProvider.CreateAsyncScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        var provider = scope.ServiceProvider.GetRequiredService<ITopicProducerProvider>();

        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        while (!stoppingToken.IsCancellationRequested)
        {
            //var messages = await dbContext.Outbox
            //    .Where(c => c.CreatedAt >= lastUpdate)
            //    .ToArrayAsync(stoppingToken);

            //if (messages is { Length: > 0 })
            //{
                //foreach (var message in messages)
                //{
                //    var payload = JObject.Parse(message.Payload);
                //    var topic = payload.GetValue("Topic");
                //    if (topic is null or { Type: not JTokenType.String })
                //    {
                //        continue;
                //    }

                //    var messageKafka = new Message<Null, string>
                //    {
                //        Value = message.Payload
                //    };

                //    var topicName = topic.ToString();
                //    await producer.ProduceAsync(topicName, messageKafka, stoppingToken);
                //}

                //var producer = provider.GetProducer<ProductCreatedEvent>(new Uri("topic:test-topic"));
                //foreach (var message in messages)
                //{ var data = JsonSerializer.Deserialize<ProductCreatedEvent>(message.Payload);

                //   
                //    await producer.Produce(new ProductCreatedEvent()
                //    {
                //        Id = Guid.NewGuid()
                //    }, stoppingToken);
                //}
            //}

            //var producer = provider.GetProducer<ProductCreatedEvent>(new Uri("topic:test-topic"));
            //await producer.Produce(new ProductCreatedEvent()
            //{
            //    Id = Guid.NewGuid()
            //}, stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
