using MassTransit;
using Product.Core.Events;

namespace Product.Api.Consumers;

public sealed class ProductCreatedConsumer(ILogger<ProductCreatedConsumer> logger) : IConsumer<ProductCreatedEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        try
        {
            logger.LogInformation(
                "{name} Product created {messageId}. {now}. Offset = '{offset}', Partition = '{partition}'",
                nameof(ProductCreatedConsumer), context.Message.Id, DateTime.UtcNow, context.Offset(), context.Partition());
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail handle product created event. Offset = '{offset}', Partition = '{partition}'", context.Offset(), context.Partition());
            throw;
        }
    }
}