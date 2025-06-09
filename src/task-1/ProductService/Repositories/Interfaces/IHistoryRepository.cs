using ProductService.Models;
using ProductService.Queries;

namespace ProductService.Repositories.Interfaces;

public interface IHistoryRepository
{
    public Task CreateHistoryAsync(OrderHistoryItem item, CancellationToken cancellationToken);

    public IAsyncEnumerable<OrderHistoryItem> QueryHistoryAsync(
        QueryHistoryParameters parameters,
        CancellationToken cancellationToken);
}