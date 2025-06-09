using Google.Protobuf.WellKnownTypes;
using GrpcProductService.Protos;

namespace GrpcProductService.ExtensionServices;

public static class RecordExtensions
{
    public static ProductService.Models.HistoryPayload ToHistoryPayload(this HistoryPayload historyPayload)
    {
        return historyPayload.PayloadCase switch
        {
            HistoryPayload.PayloadOneofCase.Create => new ProductService.Models.HistoryPayload.CreateHistoryPayload(
               historyPayload.Create.CreatedAt.ToDateTime(),
               historyPayload.Create.CreatedBy),
            HistoryPayload.PayloadOneofCase.Removed => new ProductService.Models.HistoryPayload.RemovedHistoryPayload(
                historyPayload.Removed.RemovedAt.ToDateTime(),
                historyPayload.Removed.ProductId),
            HistoryPayload.PayloadOneofCase.Added => new ProductService.Models.HistoryPayload.AddedHistoryPayload(
                historyPayload.Added.AddedAt.ToDateTime(),
                historyPayload.Added.ProductId,
                historyPayload.Added.Quantity),
            HistoryPayload.PayloadOneofCase.Changed => new ProductService.Models.HistoryPayload.ChangedHistoryPayload(
                historyPayload.Changed.ChangedAt.ToDateTime(),
                historyPayload.Changed.OrderId),
            HistoryPayload.PayloadOneofCase.None => throw new ArgumentException("Payload is wrong type", nameof(historyPayload)),
            _ => throw new ArgumentOutOfRangeException(nameof(historyPayload), historyPayload, null),
        };
    }

    public static HistoryPayload ToGrpcHistoryPayload(this ProductService.Models.HistoryPayload historyPayload)
    {
        return historyPayload switch
        {
            ProductService.Models.HistoryPayload.CreateHistoryPayload create => new HistoryPayload
            {
                Create = new CreateHistoryPayload
                {
                    CreatedAt = Timestamp.FromDateTime(create.CreatedAt.ToUniversalTime()),
                    CreatedBy = create.CreatedBy,
                },
            },
            ProductService.Models.HistoryPayload.RemovedHistoryPayload removed => new HistoryPayload
            {
                Removed = new RemovedHistoryPayload
                {
                    RemovedAt = Timestamp.FromDateTime(removed.RemovedAt.ToUniversalTime()),
                    ProductId = removed.ProductId,
                },
            },
            ProductService.Models.HistoryPayload.AddedHistoryPayload added => new HistoryPayload
            {
                Added = new AddedHistoryPayload
                {
                    AddedAt = Timestamp.FromDateTime(added.AddedAt.ToUniversalTime()),
                    ProductId = added.ProductId,
                    Quantity = added.Quantity,
                },
            },
            ProductService.Models.HistoryPayload.ChangedHistoryPayload changed => new HistoryPayload
            {
                Changed = new ChangedHistoryPayload
                {
                    ChangedAt = Timestamp.FromDateTime(changed.AddedAt.ToUniversalTime()),
                    OrderId = changed.OrderId,
                },
            },
            _ => throw new ArgumentException("Payload is wrong type", nameof(historyPayload)),
        };
    }

    public static ProductService.Records.RequestResult ToRequestResult(this RequestResult result)
    {
        return result.ResultCase switch
        {
            RequestResult.ResultOneofCase.Failure => new ProductService.Records.RequestResult.Failure(result.Failure
                .Message),
            RequestResult.ResultOneofCase.Success => new ProductService.Records.RequestResult.Success(
                result.Success.Message,
                result.Success.Id),
            RequestResult.ResultOneofCase.None => throw new ArgumentException("Result is wrong type", nameof(result)),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null),
        };
    }

    public static RequestResult ToRequestResult(this ProductService.Records.RequestResult result)
    {
        return result switch
        {
            ProductService.Records.RequestResult.Failure failure=> new RequestResult
            {
                Failure = new Failure
                {
                    Message = failure.Message,
                },
            },
            ProductService.Records.RequestResult.Success success => new RequestResult
            {
                Success = new Success
                {
                    Message = success.Message,
                    Id = success.Id,
                },
            },
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null),
        };
    }
}