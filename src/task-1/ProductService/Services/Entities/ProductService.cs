using ProductService.Records;
using ProductService.Repositories.Interfaces;
using ProductService.Services.Interfaces;
using Product = ProductService.Models.Product;

namespace ProductService.Services.Entities;

public class ProductService : IProductService
{
    private readonly IProductsRepository _productsRepository;

    public ProductService(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task<RequestResult> CreateProductAsync(global::ProductService.Records.Product product, CancellationToken cancellationToken)
    {
        var dtoObject = new Product
        {
            ProductName = product.Name,
            ProductPrice = product.Price,
        };

        long id = await _productsRepository.CreateProductAsync(dtoObject, cancellationToken);
        return new RequestResult.Success($"Product {product.Name} created", id);
    }
}