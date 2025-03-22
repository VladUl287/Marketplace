using MediatR;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Commands;
using Product.Api.Queries;

namespace Product.Api.Controllers;

[ApiController, Route("[controller]/[action]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var result = await mediator.Send(new GetAllProductsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest command)
    {
        var result = await mediator.Send(command);
        //return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        //var result = await mediator.Send(new DeleteProductCommand());
        //return result ? NoContent() : NotFound();
        return NoContent();
    }
}
