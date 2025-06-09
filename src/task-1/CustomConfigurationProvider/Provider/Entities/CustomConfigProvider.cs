using Microsoft.Extensions.Configuration;

namespace CustomConfigurationProvider.Provider.Entities;

public class CustomConfigProvider : ConfigurationProvider
{
    public void UpdateConfiguration(IDictionary<string, string?> configuration)
    {
        bool isChanged = false;

        if (configuration.Count != 0)
        {
            configuration =
                configuration.ToDictionary(x => "Persistence:Postgres:" + x.Key, x => x.Value);
        }

        foreach (KeyValuePair<string, string?> dataConfig in Data)
        {
            if (configuration.ContainsKey(dataConfig.Key)) continue;
            Data.Remove(dataConfig.Key);
            isChanged = true;
        }

        foreach (KeyValuePair<string, string?> config in configuration)
        {
            if (Data.TryGetValue(config.Key, out string? value))
            {
                if (value == config.Value) continue;
                Data[config.Key] = config.Value;
                isChanged = true;
            }
            else
            {
                Data[config.Key] = config.Value;
                isChanged = true;
            }
        }

        if (isChanged)
            OnReload();
    }
}