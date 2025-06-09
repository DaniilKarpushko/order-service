namespace ProductServiceGateway.Requests;

public class FinishDeliveringRequest
{
    public bool IsSuccessful { get; init; }

    public string? FailureMessage { get; init; }
}