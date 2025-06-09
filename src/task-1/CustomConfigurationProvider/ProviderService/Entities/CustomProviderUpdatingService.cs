using ConfigurationLoaderService.Interfaces;
using CustomConfigurationProvider.Options;
using CustomConfigurationProvider.Provider.Entities;
using CustomConfigurationProvider.ProviderService.Interfaces;
using Microsoft.Extensions.Options;

namespace CustomConfigurationProvider.ProviderService.Entities;

public class CustomProviderUpdatingService : IProviderUpdatingService
{
    private readonly IConfigurationClient _configurationClient;
    private readonly CustomConfigProvider _configurationProvider;
    private readonly int _timePeriod;

    public CustomProviderUpdatingService(
        IConfigurationClient configurationClient,
        IOptions<ServiceOptions> options,
        CustomConfigProvider configurationProvider)
    {
        _configurationClient = configurationClient;
        _configurationProvider = configurationProvider;
        _timePeriod = options.Value.TimerTime;
    }

    public async Task RunServiceAsync(
        CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(_timePeriod));

        do
        {
            await LoadConfigAsync(cancellationToken);
        }
        while (await timer.WaitForNextTickAsync(cancellationToken));
    }

    public async Task LoadConfigAsync(CancellationToken cancellationToken)
    {
        var newConfigurations = new Dictionary<string, string?>();

        await foreach (KeyValuePair<string, string> configItem in _configurationClient.LoadConfigurationAsync(cancellationToken))
        {
            newConfigurations.Add(configItem.Key, configItem.Value);
        }

        _configurationProvider.UpdateConfiguration(newConfigurations);
    }
}