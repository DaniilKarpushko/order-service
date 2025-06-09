using Itmo.Dev.Platform.Common.Extensions;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using System.Threading.Channels;
using Kafka.Consumer;
using Kafka.Options;

namespace GrpcProductService.Services.Entities;

public class OrderMessageHandler : IMessageHandler<OrderProcessingKey, OrderProcessingValue>
{
    private readonly IProcessingStateHistoryService _processingStateHistoryService;

    private readonly int _chunkSize;
    private readonly double _batchTimeout;

    public OrderMessageHandler(
        IOptions<BatchingOptions> batchingOptions,
        IProcessingStateHistoryService processingStateHistoryService)
    {
        _processingStateHistoryService = processingStateHistoryService;

        _chunkSize = batchingOptions.Value.ChunkSize;
        _batchTimeout = batchingOptions.Value.BatchTimeout;
    }

    public async Task HandleMessageAsync(
        ChannelReader<KeyValuePair<OrderProcessingKey, OrderProcessingValue>> channelReader,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            IAsyncEnumerable<IReadOnlyList<KeyValuePair<OrderProcessingKey, OrderProcessingValue>>> messages = channelReader
                .ReadAllAsync(cancellationToken)
                .ChunkAsync(_chunkSize, TimeSpan.FromMilliseconds(_batchTimeout));

            await foreach (IReadOnlyList<KeyValuePair<OrderProcessingKey, OrderProcessingValue>> batch in messages)
            {
                await HandleBatchAsync(batch, cancellationToken);
            }
        }
    }

    private async Task HandleBatchAsync(
        IReadOnlyList<KeyValuePair<OrderProcessingKey, OrderProcessingValue>> list,
        CancellationToken cancellationToken)
    {
        foreach (KeyValuePair<OrderProcessingKey, OrderProcessingValue> state in list)
        {
            await _processingStateHistoryService.ProcessState(state.Value, cancellationToken);
        }
    }
}