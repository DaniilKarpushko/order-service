using System.Threading.Channels;

namespace Kafka.Consumer;

public interface IMessageHandler<TKey, TValue>
{
    public Task HandleMessageAsync(
        ChannelReader<KeyValuePair<TKey, TValue>> channelReader,
        CancellationToken cancellationToken);
}