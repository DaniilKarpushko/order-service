namespace CustomConfigurationProvider.ProviderService.Interfaces;

public interface IProviderUpdatingService
{
    public Task RunServiceAsync(CancellationToken cancellationToken);

    public Task LoadConfigAsync(CancellationToken cancellationToken);
}