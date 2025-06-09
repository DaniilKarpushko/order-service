namespace Kafka.Options;

public class BatchingOptions
{
    public int ChunkSize { get; set; }

    public int BatchTimeout { get; set; }
}