using Microsoft.AspNetCore.Mvc;
using ProductService.Records;
using ProductServiceGateway.Services;
using ProductServiceHttpGateway.Services;

namespace ProductServiceGateway.Controllers;

[ApiController]
[Route("gateway-api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderClientService _orderServiceGrpcClient;

    public OrderController(IOrderClientService orderServiceGrpcClient)
    {
        _orderServiceGrpcClient = orderServiceGrpcClient;
    }

    [HttpPost("order/{userName}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResult>> CreateOrderAsync(
        [FromRoute] string userName,
        CancellationToken cancellationToken)
    {
        return Ok(await _orderServiceGrpcClient.CreateOrderAsync(userName, cancellationToken));
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
        return Ok(await _orderServiceGrpcClient.AddProductAsync(orderId, productId, quantity, cancellationToken));
    }

    [HttpPut("{orderId}/process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResult>> SetOrderStateProcessingAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        return Ok(await _orderServiceGrpcClient.SetOrderStateProcessingAsync(orderId, cancellationToken));
    }

    [HttpPut("{orderId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResult>> SetOrderStateCancelledAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        return Ok(await _orderServiceGrpcClient.SetOrderStateCancelledAsync(orderId, cancellationToken));
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

        OrderHistory[] orderHistories = await _orderServiceGrpcClient
            .GetOrderHistoryAsync(orderId, limit, cancellationToken)
            .ToArrayAsync(cancellationToken);

        return Ok(orderHistories);
    }
}