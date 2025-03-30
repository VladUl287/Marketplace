using FastEndpoints;
using Product.Api.DTOs;
using Product.Core.Entities;
using Product.Core.Events;
using Product.Infrastructure.Data;
using System.Text.Json;

namespace Product.Api.Commands;

//public sealed class CreateProductCommand(ProductsDbContext dbContext) : Endpoint<CreateProductRequest, ProductDTO>
//{
//    public override void Configure()
//    {
//        Get("/product/create");
//        AllowAnonymous();
//    }

//    public override async Task HandleAsync(CreateProductRequest request, CancellationToken token)
//    {
//        var productDto = new ProductDTO
//        {
//            Id = Guid.CreateVersion7()
//        };

//        var outbox = new Outbox
//        {
//            Payload = JsonSerializer.Serialize(new ProductCreatedEvent
//            {
//                Id = productDto.Id
//            })
//        };
//        await dbContext.Outbox.AddAsync(outbox, token);
//        outbox = new Outbox
//        {
//            Payload = JsonSerializer.Serialize(new ProductUpdatedEvent
//            {
//                Id = productDto.Id
//            })
//        };
//        await dbContext.Outbox.AddAsync(outbox, token);

//        await dbContext.SaveChangesAsync(token);

//        await SendAsync(productDto, cancellation: token);
//    }
//}