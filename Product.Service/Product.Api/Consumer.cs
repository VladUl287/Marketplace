using MassTransit;
using Product.Core.Events;

namespace Product.Api;

public sealed class KafkaMessageConsumer(ILogger<KafkaMessageConsumer> logger) : IConsumer<ProductCreatedEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        logger.LogInformation($"Product created {context.Message.Id}. {DateTime.UtcNow}");

        return Task.CompletedTask;
    }
}
