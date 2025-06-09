using Microsoft.AspNetCore.Mvc;
using ProductService.Records;
using ProductService.Services.Interfaces;

namespace OrderWebApplication.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("order/{userName}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResult>> CreateOrderAsync(
        [FromRoute] string userName,
        CancellationToken cancellationToken)
    {
        RequestResult result = await _orderService.CreateOrderAsync(userName, cancellationToken);

        return CreatedAtAction(nameof(CreateOrderAsync), result);
    }

    [HttpPut("{orderId}/products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResult>> AddProductAsync(
        [FromRoute] long orderId,
        [FromQuery] long productId,
        [FromQuery] int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            return BadRequest("Wrong quantity.");

        RequestResult result = await _orderService.AddProductAsync(orderId, productId, quantity, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{orderId}/process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResult>> SetOrderStateProcessingAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        RequestResult result = await _orderService.SetOrderStateProcessingAsync(orderId, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{orderId}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResult>> SetOrderStateCompletedAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        RequestResult result = await _orderService.SetOrderStateCompletedAsync(orderId, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{orderId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResult>> SetOrderStateCancelledAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        RequestResult result = await _orderService.SetOrderStateCancelledAsync(orderId, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{orderId}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderHistory[]>> GetOrderHistoryAsync(
        [FromRoute] long orderId,
        [FromQuery] int limit,
        CancellationToken cancellationToken)
    {
        if (limit <= 0)
            return BadRequest("Wrong limit.");

        OrderHistory[] histories = await _orderService
            .GetOrderHistoryAsync(orderId, limit, cancellationToken)
            .ToArrayAsync(cancellationToken);

        return Ok(histories);
    }
}