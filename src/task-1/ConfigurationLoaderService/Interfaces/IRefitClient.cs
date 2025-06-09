using ConfigurationLoaderService.Models;
using Refit;

namespace ConfigurationLoaderService.Interfaces;

public interface IRefitClient
{
    [Get("/configurations?pageSize={pageSize}&pageToken={pageToken}")]
    Task<QueryConfigurationsResponse> GetConfiguration(int pageSize, string? pageToken, CancellationToken cancellationToken);
}