using ProductService.Models;
using ProductService.Queries;

namespace ProductService.Repositories.Interfaces;

public interface IProductsRepository
{
    public Task<long> CreateProductAsync(Product product, CancellationToken cancellationToken);

    public IAsyncEnumerable<Product> QueryProductAsync(
        QueryProductParameters parameters,
        CancellationToken cancellationToken);
}