namespace ConfigurationLoaderService.Interfaces;

public interface IConfigurationClient
{
    public IAsyncEnumerable<KeyValuePair<string, string>> LoadConfigurationAsync(CancellationToken cancellationToken);
}