namespace ProductServiceGateway.Requests;

public class ApproveRequest
{
    public bool IsSuccessful { get; init; }

    public string ApprovedBy { get; init; } = string.Empty;

    public string? FailureMessage { get; init; }
}