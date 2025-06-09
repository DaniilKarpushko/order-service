using FluentMigrator.Runner;

namespace OrderWebApplication.BackgroundServices;

public class MigrationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceCollection;

    public MigrationBackgroundService(IServiceProvider serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunMigrationsAsync(stoppingToken);
    }

    private async Task RunMigrationsAsync(CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = _serviceCollection.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}