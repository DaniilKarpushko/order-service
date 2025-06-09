using System.Runtime.CompilerServices;
using Grpc.Core;
using GrpcProductService.ExtensionServices;
using GrpcProductService.Protos;
using ProductService.Records;
using RequestResult = ProductService.Records.RequestResult;

namespace ProductServiceGateway.Services.Entities;

public class OrderClientService : IOrderClientService
{
    private readonly MyOrderService.MyOrderServiceClient _myOrderServiceGrpcClient;

    public OrderClientService(MyOrderService.MyOrderServiceClient myOrderServiceGrpcClient)
    {
        _myOrderServiceGrpcClient = myOrderServiceGrpcClient;
    }

    public async Task<RequestResult> CreateOrderAsync(string createdBy, CancellationToken cancellationToken)
    {
        CreateOrderResponse response =
            await _myOrderServiceGrpcClient.CreateOrderAsync(
                new CreateOrderRequest { CreatedBy = createdBy },
                cancellationToken: cancellationToken);

        return response.Result.ToRequestResult();
    }

    public async Task<RequestResult> AddProductAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken)
    {
        AddProductResponse response = await _myOrderServiceGrpcClient.AddProductAsync(
            new AddProductRequest { OrderId = orderId, ProductId = productId, Quantity = quantity },
            cancellationToken: cancellationToken);

        return response.Result.ToRequestResult();
    }

    public async Task<RequestResult> RemoveProductAsync(long orderId, long productId, CancellationToken cancellationToken)
    {
        RemoveProductResponse response = await _myOrderServiceGrpcClient.RemoveProductAsync(
            new RemoveProductRequest { OrderId = orderId, ProductId = productId },
            cancellationToken: cancellationToken);

        return response.Result.ToRequestResult();
    }

    public async Task<RequestResult> SetOrderStateProcessingAsync(long orderId, CancellationToken cancellationToken)
    {
        OrderStateResponse response = await _myOrderServiceGrpcClient.SetOrderStateProcessingAsync(
            new OrderStateRequest { OrderId = orderId },
            cancellationToken: cancellationToken);

        return response.Result.ToRequestResult();
    }

    public async Task<RequestResult> SetOrderStateCompletedAsync(long orderId, CancellationToken cancellationToken)
    {
        OrderStateResponse response = await _myOrderServiceGrpcClient.SetOrderStateCompletedAsync(
            new OrderStateRequest { OrderId = orderId },
            cancellationToken: cancellationToken);

        return response.Result.ToRequestResult();
    }

    public async Task<RequestResult> SetOrderStateCancelledAsync(long orderId, CancellationToken cancellationToken)
    {
        OrderStateResponse response = await _myOrderServiceGrpcClient.SetOrderStateCanceledAsync(
            new OrderStateRequest { OrderId = orderId },
            cancellationToken: cancellationToken);

        return response.Result.ToRequestResult();
    }

    public async IAsyncEnumerable<OrderHistory> GetOrderHistoryAsync(long orderId, int limit, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AsyncServerStreamingCall<OrderHistoryResponse> histories = _myOrderServiceGrpcClient
            .GetOrderHistory(
                new OrderHistoryRequest { OrderId = orderId, Limit = limit },
                cancellationToken: cancellationToken);

        await foreach (OrderHistoryResponse? response in histories.ResponseStream.ReadAllAsync(cancellationToken))
        {
            yield return new OrderHistory(
                response.OrderId,
                response.CreatedAt.ToDateTime(),
                response.Kind.ToOrderHistoryItemKind(),
                response.Payload.ToHistoryPayload());
        }
    }
}