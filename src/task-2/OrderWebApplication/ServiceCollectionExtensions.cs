using ConfigurationLoaderService.Extensions;
using ConfigurationLoaderService.Options;
using CustomConfigurationProvider.Options;
using Microsoft.Extensions.Options;
using Npgsql;
using ProductService.Extensions;
using ProductService.Models;
using ProductService.Options;

namespace OrderWebApplication;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
    {
        IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.AddOptions<ServiceOptions>().Bind(configuration.GetSection("ServiceSettings"));
        services.AddOptions<ClientOptions>().Bind(configuration.GetSection("ClientService"));
        services.AddConfigurationUploader();

        return services;
    }

    public static IServiceCollection AddApplicationsMigrations(this IServiceCollection serviceCollection)
    {
        IConfiguration config = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
        serviceCollection.AddOptions<DatabaseOptions>().Bind(config.GetSection("Persistence:Postgres"));
        serviceCollection.AddDatabaseMigrations();

        return serviceCollection;
    }

    public static IServiceCollection AddNpgSqlDataSourceBuilder(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(provider =>
        {
            IOptionsMonitor<DatabaseOptions> options = provider.GetRequiredService<IOptionsMonitor<DatabaseOptions>>();
            var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(options.CurrentValue.ConnectionString);
            npgsqlDataSourceBuilder.MapEnum<OrderState>(pgName: "order_state");
            npgsqlDataSourceBuilder.MapEnum<OrderHistoryItemKind>(pgName: "order_history_item_kind");

            options.OnChange(databaseOptions => npgsqlDataSourceBuilder.ConnectionStringBuilder.ConnectionString = databaseOptions.ConnectionString);
            return npgsqlDataSourceBuilder;
        });

        serviceCollection.AddSingleton<NpgsqlDataSource>(provider => provider.GetRequiredService<NpgsqlDataSourceBuilder>().Build());

        return serviceCollection;
    }
}