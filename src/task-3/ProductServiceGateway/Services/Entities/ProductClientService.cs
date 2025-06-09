using GrpcProductService.ExtensionServices;
using GrpcProductService.Protos;
using ProductService.Records;
using RequestResult = ProductService.Records.RequestResult;

namespace ProductServiceGateway.Services.Entities;

public class ProductClientService : IProductClientService
{
    private readonly GrpcProductService.Protos.ProductService.ProductServiceClient _productClient;

    public ProductClientService(GrpcProductService.Protos.ProductService.ProductServiceClient productClient)
    {
        _productClient = productClient;
    }

    public async Task<RequestResult> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        CreateProductResponse result = await _productClient.CreateProductAsync(
            new CreateProductRequest { Name = product.Name, Price = product.Price.ToMoney() },
            cancellationToken: cancellationToken);

        return result.Result.ToRequestResult();
    }
}