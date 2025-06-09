using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcProductService.ExtensionServices;
using GrpcProductService.Protos;
using ProductService.Records;
using ProductService.Services.Interfaces;
using RequestResult = ProductService.Records.RequestResult;

namespace GrpcProductService.Services.Entities;

public class MyOrderServiceGrpc : MyOrderService.MyOrderServiceBase
{
    private readonly IOrderService _orderService;

    public MyOrderServiceGrpc(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        RequestResult result = await _orderService.CreateOrderAsync(request.CreatedBy, context.CancellationToken);

        return new CreateOrderResponse { Result = result.ToRequestResult() };
    }

    public override async Task<AddProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
    {
        RequestResult result = await _orderService.AddProductAsync(
            request.OrderId,
            request.ProductId,
            request.Quantity,
            context.CancellationToken);

        return new AddProductResponse { Result = result.ToRequestResult() };
    }

    public override async Task<RemoveProductResponse> RemoveProduct(RemoveProductRequest request, ServerCallContext context)
    {
        RequestResult result = await _orderService.RemoveProductAsync(
            request.OrderId,
            request.ProductId,
            context.CancellationToken);

        return new RemoveProductResponse { Result = result.ToRequestResult() };
    }

    public override async Task<OrderStateResponse> SetOrderStateProcessing(OrderStateRequest request, ServerCallContext context)
    {
        RequestResult result = await _orderService.SetOrderStateProcessingAsync(request.OrderId, context.CancellationToken);

        return new OrderStateResponse { Result = result.ToRequestResult() };
    }

    public override async Task<OrderStateResponse> SetOrderStateCompleted(OrderStateRequest request, ServerCallContext context)
    {
        RequestResult result = await _orderService.SetOrderStateCompletedAsync(request.OrderId, context.CancellationToken);

        return new OrderStateResponse { Result = result.ToRequestResult() };
    }

    public override async Task<OrderStateResponse> SetOrderStateCanceled(OrderStateRequest request, ServerCallContext context)
    {
        RequestResult result = await _orderService.SetOrderStateCancelledAsync(request.OrderId, context.CancellationToken);

        return new OrderStateResponse { Result = result.ToRequestResult() };
    }

    public override async Task GetOrderHistory(
        OrderHistoryRequest request,
        IServerStreamWriter<OrderHistoryResponse> responseStream,
        ServerCallContext context)
    {
        await foreach (OrderHistory history in _orderService.GetOrderHistoryAsync(request.OrderId, request.Limit, context.CancellationToken))
        {
            ArgumentNullException.ThrowIfNull(history.OrderHistoryItemPayload);

            var response = new OrderHistoryResponse
            {
                OrderId = history.OrderId,
                CreatedAt = Timestamp.FromDateTime(history.OrderHistoryCreatedAt.ToUniversalTime()),
                Kind = history.OrderHistoryItemKind.ToGrpcOrderHistoryItemKind(),
                Payload = history.OrderHistoryItemPayload.ToGrpcHistoryPayload(),
            };
            await responseStream.WriteAsync(response);
        }
    }
}