using MassTransit;
using Product.Core.Events;

namespace Product.Api;

public sealed class ConsumerSecond(ILogger<ConsumerSecond> logger) : IConsumer<ProductCreatedEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        logger.LogInformation($"{nameof(ConsumerSecond)} Product created {context.Message.Id}. {DateTime.UtcNow}");

        return Task.CompletedTask;
    }
}