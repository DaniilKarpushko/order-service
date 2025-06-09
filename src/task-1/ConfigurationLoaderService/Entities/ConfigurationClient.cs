using ConfigurationLoaderService.Interfaces;
using ConfigurationLoaderService.Models;
using ConfigurationLoaderService.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConfigurationLoaderService.Entities;

public class ConfigurationClient : IConfigurationClient
{
    private readonly HttpClient _client;
    private int _pageSize;

    public ConfigurationClient(IHttpClientFactory clientFactory, IOptionsMonitor<ClientOptions> optionsMonitor)
    {
        _client = clientFactory.CreateClient("ConfigurationClient");
        _pageSize = optionsMonitor.CurrentValue.PageSize;
        optionsMonitor.OnChange(x => _pageSize = x.PageSize);
    }

    public async IAsyncEnumerable<KeyValuePair<string, string>> LoadConfigurationAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string? pageToken = null;
        var stringBuilder = new StringBuilder();
        do
        {
            stringBuilder.Clear();
            stringBuilder.Append($"configurations?pageSize={_pageSize}");
            if (pageToken is not null)
            {
                stringBuilder.Append($"&pageToken={pageToken}");
            }

            HttpResponseMessage response =
                await _client.GetAsync(stringBuilder.ToString(), cancellationToken);
            if (!response.IsSuccessStatusCode)
                continue;

            QueryConfigurationsResponse configurationsResponse = await response.Content.ReadFromJsonAsync<QueryConfigurationsResponse>(cancellationToken) ?? throw new Exception("Null response");
            foreach (ConfigurationItemDto dtos in configurationsResponse.Items)
            {
                yield return new KeyValuePair<string, string>(dtos.Key, dtos.Value);
            }

            pageToken = configurationsResponse.PageToken;
        }
        while (pageToken is not null);
    }
}