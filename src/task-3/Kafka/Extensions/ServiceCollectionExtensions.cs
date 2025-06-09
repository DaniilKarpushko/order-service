using Kafka.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<KafkaConsumerOptions>().Bind(configuration.GetSection("KafkaConsumer"));
        serviceCollection.AddOptions<BatchingOptions>().Bind(configuration.GetSection("Batching"));
        serviceCollection.AddOptions<KafkaProducerOptions>().Bind(configuration.GetSection("KafkaProducer"));
        serviceCollection.AddOptions<KafkaOptions>().Bind(configuration.GetSection("Kafka"));

        return serviceCollection;
    }
}