using Microsoft.AspNetCore.Mvc;
using ProductService.Records;
using ProductService.Services.Interfaces;

namespace OrderWebApplication.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("product")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResult>> CreateProductAsync(
        [FromQuery] string name,
        [FromQuery] decimal price,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name) || price <= 0)
            return BadRequest("Name and price must be greater than 0.");

        var product = new Product(name, price);
        RequestResult result = await _productService.CreateProductAsync(product, cancellationToken);

        return CreatedAtAction(nameof(CreateProductAsync), result.ToString(), product);
    }
}