using Mapster;
using MassTransit;
using MediatR;
using Product.Api.DTOs;
using Product.Core.Entities;
using Product.Core.Events;
using Product.Infrastructure.Data;
using System.Text.Json;

namespace Product.Api.Commands;

public sealed class CreateProductCommand(ITopicProducerProvider topicProducerProvider, ProductsDbContext dbContext) : IRequestHandler<CreateProductRequest, ProductDTO>
{
    public async Task<ProductDTO> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        //var producer = topicProducerProvider.GetProducer<ProductCreatedEvent>(new Uri("test-topic"));

        var productDto = new ProductDTO
        {
            Id = Guid.CreateVersion7()
        };

        //await producer.Produce(product, cancellationToken);

        //var product = productDto.Adapt<Commodity>();
        //await dbContext.Products.AddAsync(product, cancellationToken);

        var outbox = new Outbox
        {
            Payload = JsonSerializer.Serialize(new ProductCreatedEvent
            {
                Id = productDto.Id
            })
        };
        await dbContext.Outbox.AddAsync(outbox, cancellationToken);
        outbox = new Outbox
        {
            Payload = JsonSerializer.Serialize(new ProductUpdatedEvent
            {
                Id = productDto.Id
            })
        };
        await dbContext.Outbox.AddAsync(outbox, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return productDto;
    }
}
