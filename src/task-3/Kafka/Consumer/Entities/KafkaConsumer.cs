using System.Threading.Channels;
using Confluent.Kafka;
using Kafka.Options;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer.Entities;

public sealed class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> _consumer;

    public KafkaConsumer(
        IOptions<KafkaConsumerOptions> kafkaConsumerOptions,
        IOptions<KafkaOptions> kafkaOptions,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer)
    {
        var consumerConfiguration = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.BootstrapServers,
            GroupId = kafkaOptions.Value.GroupId,
        };

        _consumer = new ConsumerBuilder<TKey, TValue>(consumerConfiguration)
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();

        _consumer.Subscribe(kafkaConsumerOptions.Value.Topic);
    }

    public async Task ConsumeAsync(
        ChannelWriter<KeyValuePair<TKey, TValue>> channelWriter,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                ConsumeResult<TKey, TValue> consumeResult = _consumer.Consume(cancellationToken);
                var message = new KeyValuePair<TKey, TValue>(consumeResult.Message.Key, consumeResult.Message.Value);

                await channelWriter.WriteAsync(message, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"KafkaConsumer: {e.Message}.");
            }
        }
    }
}