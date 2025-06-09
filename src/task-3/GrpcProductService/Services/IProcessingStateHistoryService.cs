using Orders.Kafka.Contracts;

namespace GrpcProductService.Services;

public interface IProcessingStateHistoryService
{
    public Task ProcessState(OrderProcessingValue processingValue, CancellationToken cancellationToken);
}