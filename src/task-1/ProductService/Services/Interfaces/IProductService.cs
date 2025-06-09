using ProductService.Records;

namespace ProductService.Services.Interfaces;

public interface IProductService
{
    public Task<RequestResult> CreateProductAsync(Product product, CancellationToken cancellationToken);
}