namespace Kafka.Producer;

public interface IKafkaProducer<TKey, TValue>
{
    public Task SendMessageAsync(string topic, TKey key, TValue value, CancellationToken cancellationToken);
}