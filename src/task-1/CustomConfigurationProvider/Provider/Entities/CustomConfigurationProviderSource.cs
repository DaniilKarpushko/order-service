using Microsoft.Extensions.Configuration;

namespace CustomConfigurationProvider.Provider.Entities;

public class CustomConfigurationProviderSource : IConfigurationSource
{
    private readonly ConfigurationProvider _provider;

    public CustomConfigurationProviderSource(ConfigurationProvider provider)
    {
        _provider = provider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _provider;
    }
}