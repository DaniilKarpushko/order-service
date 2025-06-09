namespace Kafka.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;
}