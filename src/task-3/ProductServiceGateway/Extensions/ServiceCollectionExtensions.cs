using Microsoft.OpenApi.Models;
using ProductServiceGateway.Options;
using ProductServiceGateway.Services;
using ProductServiceGateway.Services.Entities;
using ProductServiceHttpGateway.Services;
using ProductServiceHttpGateway.Services.Entities;

namespace ProductServiceGateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClientOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GrpcClientOptions>(configuration.GetSection("GrpcClient"));
        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderAPI", Version = "v1" });
            c.UseOneOfForPolymorphism();
        });
        return services;
    }

    public static IServiceCollection AddGrpcClientServices(this IServiceCollection services)
    {
        services.AddSingleton<IProductClientService, ProductClientService>();
        services.AddSingleton<IOrderClientService, OrderClientService>();
        services.AddSingleton<IProcessClientService, ProcessClientService>();

        return services;
    }
}