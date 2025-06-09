namespace ProductServiceHttpGateway.Services;

public interface IProcessClientService
{
    public Task ApproveOrderAsync(long orderId, bool isSuccessful, string approvedBy, string? failureMessage);

    public Task StartOrderPackingAsync(long orderId, string packedBy);

    public Task FinishOrderPackingAsync(long orderId, bool isSuccessful, string? failureMessage);

    public Task StartOrderDeliveringAsync(long orderId, string deliveredBy);

    public Task FinishOrderDeliveryAsync(long orderId, bool isSuccessful, string? failureMessage);
}