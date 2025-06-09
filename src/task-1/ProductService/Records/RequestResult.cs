namespace ProductService.Records;

public record RequestResult
{
    public sealed record Success(string Message, long Id) : RequestResult;

    public sealed record Failure(string Message) : RequestResult;
}