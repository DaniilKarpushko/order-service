using Microsoft.AspNetCore.Mvc;
using ProductServiceGateway.Requests;
using ProductServiceHttpGateway.Services;

namespace ProductServiceHttpGateway.Controllers;

[ApiController]
[Route("gateway-api/orders-process-state/")]
public class ProcessController : ControllerBase
{
    private readonly IProcessClientService _processClientService;

    public ProcessController(IProcessClientService processClientService)
    {
        _processClientService = processClientService;
    }

    [HttpPost("approve/{orderId}")]
    public async Task<ActionResult> ApproveOrder(
        [FromRoute] long orderId,
        [FromBody] ApproveRequest approveRequest)
    {
        await _processClientService.ApproveOrderAsync(
                orderId,
                approveRequest.IsSuccessful,
                approveRequest.ApprovedBy,
                approveRequest.FailureMessage);

        return Ok();
    }

    [HttpPut("start-packing/{orderId}")]
    public async Task<ActionResult> StartPacking([FromRoute] long orderId, [FromQuery] string packingBy)
    {
        await _processClientService.StartOrderPackingAsync(orderId, packingBy);

        return Ok();
    }

    [HttpPut("finish-packing/{orderId}")]
    public async Task<ActionResult> FinishPacking(
        [FromRoute] long orderId,
        [FromBody] FinishPackingRequest finishPackingRequest)
    {
        await _processClientService.FinishOrderPackingAsync(
            orderId,
            finishPackingRequest.IsSuccessful,
            finishPackingRequest.FailureMessage);

        return Ok();
    }

    [HttpPut("start-delivering/{orderId}")]
    public async Task<ActionResult> StartDelivering([FromRoute] long orderId, [FromQuery] string deliveringBy)
    {
        await _processClientService.StartOrderDeliveringAsync(orderId, deliveringBy);

        return Ok();
    }

    [HttpPut("finish-delivery/{orderId}")]
    public async Task<ActionResult> FinishDelivering(
        [FromRoute] long orderId,
        [FromBody] FinishDeliveringRequest finishDeliveringRequest)
    {
        await _processClientService.FinishOrderDeliveryAsync(
            orderId,
            finishDeliveringRequest.IsSuccessful,
            finishDeliveringRequest.FailureMessage);

        return Ok();
    }
}