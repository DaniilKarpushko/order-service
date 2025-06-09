using ProductService.Records;

namespace ProductServiceGateway.Services;

public interface IProductClientService
{
    public Task<RequestResult> CreateProductAsync(Product product, CancellationToken cancellationToken);
}