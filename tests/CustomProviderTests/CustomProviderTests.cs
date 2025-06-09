using System.Collections.Generic;
using CustomConfigurationProvider.Provider.Entities;
using FluentAssertions;

namespace CustomProviderTests;

public class CustomProviderTests
{
    [Fact]
    public void EmptyProvider_NewDataTest_ReturnsUpdateProvider()
    {
        var config = new Dictionary<string, string?>() { { "Persistence", "Postgres" }, { "Host", "localhost" } };
        var configProvider = new CustomConfigProvider();
        Microsoft.Extensions.Primitives.IChangeToken token = configProvider.GetReloadToken();
        configProvider.UpdateConfiguration(config);

        token.HasChanged.Should().BeTrue();
    }

    [Fact]
    public void Provider_SameDataTest_ReturnsSameProvider()
    {
        var config = new Dictionary<string, string?>() { { "Persistence", "Postgres" }, { "Host", "localhost" } };
        var configProvider = new CustomConfigProvider();
        configProvider.UpdateConfiguration(config);

        Microsoft.Extensions.Primitives.IChangeToken token = configProvider.GetReloadToken();

        configProvider.UpdateConfiguration(config);
        token.HasChanged.Should().BeFalse();
    }

    [Fact]
    public void Provider_DiffDataTest_ReturnsProviderWithNewValue()
    {
        var config = new Dictionary<string, string?>() { { "Persistence", "Postgres" }, { "Host", "localhost" } };
        var configProvider = new CustomConfigProvider();
        configProvider.UpdateConfiguration(config);
        Microsoft.Extensions.Primitives.IChangeToken token = configProvider.GetReloadToken();

        var newConfig = new Dictionary<string, string?>() { { "Persistence", "Postgres" }, { "Host", "www.host.com" } };
        configProvider.UpdateConfiguration(newConfig);
        token.HasChanged.Should().BeTrue();
    }

    [Fact]
    public void Provider_EmptyDataTest_ReturnsEmptyProvider()
    {
        var config = new Dictionary<string, string?>() { { "Persistence", "Postgres" }, { "Host", "localhost" } };
        var configProvider = new CustomConfigProvider();

        configProvider.UpdateConfiguration(config);
        Microsoft.Extensions.Primitives.IChangeToken token = configProvider.GetReloadToken();

        var newConfig = new Dictionary<string, string?>();
        configProvider.UpdateConfiguration(newConfig);
        token.HasChanged.Should().BeTrue();
    }
}