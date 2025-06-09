using ConfigurationLoaderService.Extensions;
using CustomConfigurationProvider.Provider.Entities;
using CustomConfigurationProvider.ProviderService.Entities;
using CustomConfigurationProvider.ProviderService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CustomConfigurationProvider.Extensions;

public static class ServiceCollectionExtensionsClientService
{
    public static IServiceCollection AddClientService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(new CustomConfigProvider());
        serviceCollection.AddConfigurationUploader();
        serviceCollection.AddScoped<IProviderUpdatingService, CustomProviderUpdatingService>();
        return serviceCollection;
    }
}