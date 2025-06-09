using ProductService.Models;
using ProductService.Queries;

namespace ProductService.Repositories.Interfaces;

public interface IOrderPositionRepository
{
    public Task AddOrderPositionAsync(OrderPosition orderPosition, CancellationToken cancellationToken);

    public Task SoftRemoveOrderPositionAsync(long id, long productId, CancellationToken cancellationToken);

    public IAsyncEnumerable<OrderPosition> QueryOrderPositionAsync(
        QueryOrderPositionParameters parameters,
        CancellationToken cancellationToken);
}