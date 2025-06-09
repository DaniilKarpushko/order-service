namespace ProductServiceGateway.Requests;

public class FinishPackingRequest
{
    public bool IsSuccessful { get; init; }

    public string? FailureMessage { get; init; }
}