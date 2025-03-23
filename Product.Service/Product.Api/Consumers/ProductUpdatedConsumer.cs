using MassTransit;
using Product.Core.Events;

namespace Product.Api.Consumers;

public sealed class ProductUpdatedConsumer(ILogger<ProductUpdatedEvent> logger) : IConsumer<ProductUpdatedEvent>
{
    public Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        logger.LogInformation("Process update product with id = '{id}'", context.Message.Id);

        return Task.CompletedTask;
    }
}
