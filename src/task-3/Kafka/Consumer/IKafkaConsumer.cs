using System.Threading.Channels;

namespace Kafka.Consumer;

public interface IKafkaConsumer<TKey, TValue>
{
    public Task ConsumeAsync(
        ChannelWriter<KeyValuePair<TKey, TValue>> channelWriter,
        CancellationToken cancellationToken);
}