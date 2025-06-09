using ConfigurationLoaderService.Entities;
using ConfigurationLoaderService.Interfaces;
using ConfigurationLoaderService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace ConfigurationLoaderService.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfigurationUploader(this IServiceCollection services)
    {
        services.AddHttpClient("ConfigurationClient", (provider, client) =>
            client.BaseAddress = new Uri(provider.GetRequiredService<IOptions<ClientOptions>>().Value.BaseUrl));
        services.AddSingleton<IConfigurationClient, ConfigurationClient>();

        return services;
    }

    public static IServiceCollection AddRefitConfigurationUploader(this IServiceCollection services, IConfiguration configuration)
    {
        string baseUrl = services.BuildServiceProvider().GetRequiredService<IOptions<ClientOptions>>().Value.BaseUrl;

        services.AddRefitClient<IRefitClient>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseUrl));
        services.AddScoped<IConfigurationClient, RefitConfigurationClient>();

        return services;
    }
}