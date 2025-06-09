using Confluent.Kafka;
using Google.Protobuf;

namespace Kafka.Serializer;

public class KafkaMessageSerializer<T> : ISerializer<T>, IDeserializer<T> where T : IMessage<T>, new()
{
    private static readonly MessageParser<T> Parser = new MessageParser<T>(() => new T());

    public byte[] Serialize(T data, SerializationContext context)
    {
        return data.ToByteArray();
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return isNull is false ? Parser.ParseFrom(data) : new T();
    }
}