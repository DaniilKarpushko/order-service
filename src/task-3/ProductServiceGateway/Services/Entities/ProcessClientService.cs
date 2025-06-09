using Orders.ProcessingService.Contracts;

namespace ProductServiceHttpGateway.Services.Entities;

public class ProcessClientService : IProcessClientService
{
    private readonly OrderService.OrderServiceClient _orderServiceClient;

    public ProcessClientService(OrderService.OrderServiceClient orderServiceClient)
    {
        _orderServiceClient = orderServiceClient;
    }

    public async Task ApproveOrderAsync(long orderId, bool isSuccessful, string approvedBy, string? failureMessage)
    {
        await _orderServiceClient.ApproveOrderAsync(
            new ApproveOrderRequest
            {
                ApprovedBy = approvedBy,
                FailureReason = failureMessage,
                IsApproved = isSuccessful,
                OrderId = orderId,
            });
    }

    public async Task StartOrderPackingAsync(long orderId, string packedBy)
    {
        await _orderServiceClient.StartOrderPackingAsync(
            new StartOrderPackingRequest
            {
                OrderId = orderId,
                PackingBy = packedBy,
            });
    }

    public async Task FinishOrderPackingAsync(long orderId, bool isSuccessful, string? failureMessage)
    {
        await _orderServiceClient.FinishOrderPackingAsync(
            new FinishOrderPackingRequest
            {
                OrderId = orderId,
                FailureReason = failureMessage,
                IsSuccessful = isSuccessful,
            });
    }

    public async Task StartOrderDeliveringAsync(long orderId, string deliveredBy)
    {
        await _orderServiceClient.StartOrderDeliveryAsync(
            new StartOrderDeliveryRequest
            {
                DeliveredBy = deliveredBy,
                OrderId = orderId,
            });
    }

    public async Task FinishOrderDeliveryAsync(long orderId, bool isSuccessful, string? failureMessage)
    {
        await _orderServiceClient.FinishOrderDeliveryAsync(
            new FinishOrderDeliveryRequest
            {
                OrderId = orderId,
                FailureReason = failureMessage,
                IsSuccessful = isSuccessful,
            });
    }
}