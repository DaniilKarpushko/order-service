using ConfigurationLoaderService.Interfaces;
using ConfigurationLoaderService.Models;
using ConfigurationLoaderService.Options;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace ConfigurationLoaderService.Entities;

public class RefitConfigurationClient : IConfigurationClient
{
    private readonly IRefitClient _refitClient;
    private int _pageSize;

    public RefitConfigurationClient(IRefitClient refitClient, IOptionsMonitor<ClientOptions> optionsMonitor)
    {
        _refitClient = refitClient;
        _pageSize = optionsMonitor.CurrentValue.PageSize;
        optionsMonitor.OnChange(x => _pageSize = x.PageSize);
    }

    public async IAsyncEnumerable<KeyValuePair<string, string>> LoadConfigurationAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string? pageToken = null;
        do
        {
            QueryConfigurationsResponse data = await _refitClient.GetConfiguration(_pageSize, pageToken, cancellationToken);
            foreach (ConfigurationItemDto dtoObject in data.Items)
            {
                yield return new KeyValuePair<string, string>(dtoObject.Key, dtoObject.Value);
            }

            pageToken = data.PageToken;
        }
        while (pageToken is not null);
    }
}