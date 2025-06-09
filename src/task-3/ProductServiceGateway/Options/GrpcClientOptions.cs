namespace ProductServiceGateway.Options;

public sealed class GrpcClientOptions
{
    public string Address { get; init; } = string.Empty;

    public string ProcessingServiceAddress { get; init; } = string.Empty;
}