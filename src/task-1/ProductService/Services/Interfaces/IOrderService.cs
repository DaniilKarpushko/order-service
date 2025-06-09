using ProductService.Records;

namespace ProductService.Services.Interfaces;

public interface IOrderService
{
    public Task<RequestResult> CreateOrderAsync(string createdBy, CancellationToken cancellationToken);

    public Task<RequestResult> AddProductAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken);

    public Task<RequestResult> RemoveProductAsync(long orderId, long productId, CancellationToken cancellationToken);

    public Task<RequestResult> SetOrderStateProcessingAsync(long orderId, CancellationToken cancellationToken);

    public Task<RequestResult> SaveProcessingOrderStateAsync(long orderId, string comment, CancellationToken cancellationToken);

    public Task<RequestResult> SetOrderStateCompletedAsync(long orderId, CancellationToken cancellationToken);

    public Task<RequestResult> SetOrderStateCancelledAsync(long orderId, CancellationToken cancellationToken);

    public Task ForceSetOrderStateCancelledAsync(long orderId, CancellationToken cancellationToken);

    public IAsyncEnumerable<OrderHistory> GetOrderHistoryAsync(long orderId, int limit, CancellationToken cancellationToken);
}