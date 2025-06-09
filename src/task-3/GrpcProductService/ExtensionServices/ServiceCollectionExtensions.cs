using GrpcProductService.Services;
using GrpcProductService.Services.Entities;
using ProductService.Options;
using ProductService.Repositories.Entities;
using ProductService.Repositories.Interfaces;
using ProductService.Services.Interfaces;

namespace GrpcProductService.ExtensionServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductServicesNew(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddOptions<DatabaseOptions>().Bind(configuration.GetSection("DatabaseOptions"));
        serviceCollection.AddSingleton<IProductsRepository, ProductsRepository>();
        serviceCollection.AddSingleton<IOrderRepository, OrderRepository>();
        serviceCollection.AddSingleton<IHistoryRepository, HistoryRepository>();
        serviceCollection.AddSingleton<IOrderPositionRepository, OrderPositionRepository>();

        serviceCollection.AddScoped<IProductService, ProductService.Services.Entities.ProductService>();
        serviceCollection.AddSingleton<IOrderService, OrderServiceDecorator>();

        serviceCollection.AddSingleton<IProcessingStateHistoryService, ProcessingStateHistoryService>();

        return serviceCollection;
    }
}
