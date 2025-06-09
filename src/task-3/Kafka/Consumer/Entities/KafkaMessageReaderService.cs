using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace Kafka.Consumer.Entities;

public class KafkaMessageReaderService<TKey, TValue> : BackgroundService
{
    private readonly IKafkaConsumer<TKey, TValue> _kafkaConsumer;
    private readonly IMessageHandler<TKey, TValue> _messageHandler;

    public KafkaMessageReaderService(
        IKafkaConsumer<TKey, TValue> kafkaConsumer,
        IMessageHandler<TKey, TValue> messageHandler)
    {
        _kafkaConsumer = kafkaConsumer;
        _messageHandler = messageHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        var channel = Channel.CreateUnbounded<KeyValuePair<TKey, TValue>>();

        await Task.WhenAll(
            _messageHandler.HandleMessageAsync(channel, stoppingToken),
            _kafkaConsumer.ConsumeAsync(channel.Writer, stoppingToken));
    }
}