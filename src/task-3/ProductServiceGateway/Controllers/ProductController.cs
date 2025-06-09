using Microsoft.AspNetCore.Mvc;
using ProductService.Records;
using ProductServiceGateway.Services;
using ProductServiceHttpGateway.Services;
using RequestResult = ProductService.Records.RequestResult;

namespace ProductServiceGateway.Controllers;

[ApiController]
[Route("gateway-api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductClientService _productService;

    public ProductController(IProductClientService productService)
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
        return string.IsNullOrEmpty(name) || price <= 0
            ? BadRequest("Name and price must be greater than 0.")
            : Ok(await _productService.CreateProductAsync(
            new Product(name, price),
            cancellationToken));
    }
}