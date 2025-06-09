using ProductService.Migrations;
using ProductService.Models;
using ProductService.Options;
using ProductService.Repositories.Entities;
using ProductService.Repositories.Interfaces;
using ProductService.Services.Entities;
using ProductService.Services.Interfaces;
using ConfigurationLoaderService.Extensions;
using ConfigurationLoaderService.Options;
using CustomConfigurationProvider.Options;
using CustomConfigurationProvider.Provider.Entities;
using CustomConfigurationProvider.ProviderService.Entities;
using CustomConfigurationProvider.ProviderService.Interfaces;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ProductService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseMigrations(
        this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(provider =>
                {
                    IOptions<DatabaseOptions> options = provider.GetRequiredService<IOptions<DatabaseOptions>>();
                    return options.Value.ConnectionString;
                })
                .WithMigrationsIn(typeof(InitialMigration).Assembly));

        return serviceCollection;
    }

    public static IServiceCollection AddProductServices(
        this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProductsRepository, ProductsRepository>();
        serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
        serviceCollection.AddScoped<IHistoryRepository, HistoryRepository>();
        serviceCollection.AddScoped<IOrderPositionRepository, OrderPositionRepository>();

        serviceCollection.AddScoped<IProductService, Services.Entities.ProductService>();
        serviceCollection.AddScoped<IOrderService, OrderService>();

        return serviceCollection;
    }

    public static IServiceCollection AddMigrationOptions(this IServiceCollection serviceCollection)
    {
        IConfiguration config = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
        serviceCollection.AddOptions<DatabaseOptions>().Bind(config.GetSection("Persistence:Postgres"));
        serviceCollection.AddDatabaseMigrations();
        return serviceCollection;
    }

    public static IServiceCollection AddServiceOptions(this IServiceCollection serviceCollection)
    {
        IConfiguration config = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
        serviceCollection.AddOptions<ServiceOptions>().Bind(config.GetSection("ServiceSettings"));
        serviceCollection.AddOptions<ClientOptions>().Bind(config.GetSection("ClientService"));
        serviceCollection.AddConfigurationUploader();
        serviceCollection.AddSingleton<IProviderUpdatingService, CustomProviderUpdatingService>();

        return serviceCollection;
    }

    public static IServiceCollection AddNpgsqlDataSourceBuilder(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(provider =>
        {
            IOptionsMonitor<DatabaseOptions> options = provider.GetRequiredService<IOptionsMonitor<DatabaseOptions>>();
            var builder = new NpgsqlDataSourceBuilder(options.CurrentValue.ConnectionString);
            builder.MapEnum<OrderState>(pgName: "order_state");
            builder.MapEnum<OrderHistoryItemKind>(pgName: "order_history_item_kind");

            options.OnChange(databaseOptions => builder.ConnectionStringBuilder.ConnectionString = databaseOptions.ConnectionString);
            return builder;
        });

        serviceCollection.AddScoped(provider => provider.GetRequiredService<NpgsqlDataSourceBuilder>().Build());

        return serviceCollection;
    }

    public static IServiceCollection AddCustomConfigurationProvider(this IServiceCollection serviceCollection)
    {
        var configProvider = new CustomConfigProvider();
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appSettings.json")
            .Add(new CustomConfigurationProviderSource(configProvider));

        serviceCollection.AddSingleton(configProvider);
        serviceCollection.AddSingleton<IConfigurationBuilder>(builder);
        serviceCollection.AddTransient<IConfiguration>(provider => provider.GetRequiredService<IConfigurationBuilder>().Build());

        return serviceCollection;
    }
}