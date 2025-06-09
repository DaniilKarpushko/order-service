using Confluent.Kafka;
using Kafka.Options;
using Microsoft.Extensions.Options;

namespace Kafka.Producer.Entities;

public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;

    public KafkaProducer(
        IOptions<KafkaOptions> kafkaOptions,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.Value.BootstrapServers,
        };

        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task SendMessageAsync(string topic, TKey key, TValue value, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(topic, new Message<TKey, TValue> { Key = key, Value = value }, cancellationToken);
    }
}