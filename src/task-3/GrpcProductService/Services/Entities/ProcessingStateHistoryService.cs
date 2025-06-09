using Orders.Kafka.Contracts;
using ProductService.Services.Interfaces;

namespace GrpcProductService.Services.Entities;

public class ProcessingStateHistoryService : IProcessingStateHistoryService
{
    private readonly IOrderService _orderService;

    public ProcessingStateHistoryService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task ProcessState(OrderProcessingValue processingValue, CancellationToken cancellationToken)
    {
        switch (processingValue.EventCase)
        {
            case OrderProcessingValue.EventOneofCase.ApprovalReceived:
                await ProcessApprovalStateAsync(processingValue, cancellationToken);
                break;
            case OrderProcessingValue.EventOneofCase.PackingStarted:
                await ProcessPackingStartedStateAsync(processingValue, cancellationToken);
                break;
            case OrderProcessingValue.EventOneofCase.PackingFinished:
                await ProcessPackingFinishedStateAsync(processingValue, cancellationToken);
                break;
            case OrderProcessingValue.EventOneofCase.DeliveryStarted:
                await ProcessDeliveryStartedStateAsync(processingValue, cancellationToken);
                break;
            case OrderProcessingValue.EventOneofCase.DeliveryFinished:
                await ProcessDeliveryFinishedStateAsync(processingValue, cancellationToken);
                break;
            case OrderProcessingValue.EventOneofCase.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(processingValue));
        }
    }

    private async Task ProcessApprovalStateAsync(
        OrderProcessingValue processingValue,
        CancellationToken cancellationToken)
    {
        OrderProcessingValue.Types.OrderApprovalReceived approvalReceived = processingValue.ApprovalReceived;
        if (!approvalReceived.IsApproved)
        {
            await _orderService.ForceSetOrderStateCancelledAsync(approvalReceived.OrderId, cancellationToken);
            return;
        }

        await _orderService.SaveProcessingOrderStateAsync(approvalReceived.OrderId, "Approved", cancellationToken);
    }

    private async Task ProcessPackingStartedStateAsync(
        OrderProcessingValue processingValue,
        CancellationToken cancellationToken)
    {
        OrderProcessingValue.Types.OrderPackingStarted packingStarted = processingValue.PackingStarted;
        await _orderService.SaveProcessingOrderStateAsync(
            packingStarted.OrderId,
            $"Packing started by {packingStarted.PackingBy}",
            cancellationToken);
    }

    private async Task ProcessPackingFinishedStateAsync(
        OrderProcessingValue processingValue,
        CancellationToken cancellationToken)
    {
        OrderProcessingValue.Types.OrderPackingFinished packingFinished = processingValue.PackingFinished;
        if (!packingFinished.IsFinishedSuccessfully)
        {
            await _orderService.SaveProcessingOrderStateAsync(
                packingFinished.OrderId,
                $"{packingFinished.FailureReason}",
                cancellationToken);
            await _orderService.ForceSetOrderStateCancelledAsync(packingFinished.OrderId, cancellationToken);
        }

        await _orderService.SaveProcessingOrderStateAsync(
            packingFinished.OrderId,
            $"Packing finished by {packingFinished.FinishedAt}",
            cancellationToken);
    }

    private async Task ProcessDeliveryStartedStateAsync(
        OrderProcessingValue processingValue,
        CancellationToken cancellationToken)
    {
        OrderProcessingValue.Types.OrderDeliveryStarted deliveryStarted = processingValue.DeliveryStarted;
        await _orderService.SaveProcessingOrderStateAsync(
            deliveryStarted.OrderId,
            $"Delivery started by {deliveryStarted.DeliveredBy}",
            cancellationToken);
    }

    private async Task ProcessDeliveryFinishedStateAsync(
        OrderProcessingValue processingValue,
        CancellationToken cancellationToken)
    {
        OrderProcessingValue.Types.OrderDeliveryFinished deliveryFinished = processingValue.DeliveryFinished;
        if (!deliveryFinished.IsFinishedSuccessfully)
        {
            await _orderService.SaveProcessingOrderStateAsync(
                deliveryFinished.OrderId,
                $"{deliveryFinished.FailureReason}",
                cancellationToken);
            await _orderService.ForceSetOrderStateCancelledAsync(deliveryFinished.OrderId, cancellationToken);
        }

        await _orderService.SaveProcessingOrderStateAsync(deliveryFinished.OrderId, "Finished", cancellationToken);
        await _orderService.SetOrderStateCompletedAsync(deliveryFinished.OrderId, cancellationToken);
    }
}