using CustomConfigurationProvider.ProviderService.Interfaces;

namespace OrderWebApplication.BackgroundServices;

public class CustomProviderUpdatingBackgroundService : BackgroundService
{
    private readonly IProviderUpdatingService _providerUpdatingService;

    public CustomProviderUpdatingBackgroundService(IProviderUpdatingService providerUpdatingService)
    {
        _providerUpdatingService = providerUpdatingService;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _providerUpdatingService.LoadConfigAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _providerUpdatingService.RunServiceAsync(stoppingToken);
    }
}