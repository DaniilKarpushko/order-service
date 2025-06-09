using ProductService.Models;
using ProductService.Queries;

namespace ProductService.Repositories.Interfaces;

public interface IOrderRepository
{
    public Task<long> CreateOrderAsync(Order order, CancellationToken cancellationToken);

    public Task UpdateOrderAsync(long id, OrderState state, CancellationToken cancellationToken);

    public Task<Order> GetOrderByIdAsync(long id, CancellationToken cancellationToken);

    public IAsyncEnumerable<Order> QueryOrderAsync(
        QueryOrderParameters orderParameters,
        CancellationToken cancellationToken);
}