using Grpc.Core;
using GrpcProductService.ExtensionServices;
using GrpcProductService.Protos;
using ProductService.Records;
using ProductService.Services.Interfaces;

namespace GrpcProductService.Services.Entities;

public class ProductServiceGrpc : Protos.ProductService.ProductServiceBase
{
    private readonly IProductService _productService;

    public ProductServiceGrpc(IProductService productService)
    {
        _productService = productService;
    }

    public override async Task<CreateProductResponse> CreateProduct(
        CreateProductRequest request,
        ServerCallContext context)
    {
        ProductService.Records.RequestResult result = await _productService.CreateProductAsync(
            new Product(request.Name, request.Price.DecimalValue),
            context.CancellationToken);

        return new CreateProductResponse { Result = result.ToRequestResult() };
    }
}