using System.Text.Json.Serialization;

namespace ProductService.Models;

[JsonDerivedType(typeof(CreateHistoryPayload), typeDiscriminator: nameof(CreateHistoryPayload))]
[JsonDerivedType(typeof(RemovedHistoryPayload), typeDiscriminator: nameof(RemovedHistoryPayload))]
[JsonDerivedType(typeof(AddedHistoryPayload), typeDiscriminator: nameof(AddedHistoryPayload))]
[JsonDerivedType(typeof(ChangedHistoryPayload), typeDiscriminator: nameof(ChangedHistoryPayload))]
[JsonDerivedType(typeof(ProcessingHistoryPayload), typeDiscriminator: nameof(ProcessingHistoryPayload))]
public record HistoryPayload
{
    public sealed record CreateHistoryPayload(DateTime CreatedAt, string CreatedBy) : HistoryPayload;

    public sealed record RemovedHistoryPayload(DateTime RemovedAt, long ProductId) : HistoryPayload;

    public sealed record AddedHistoryPayload(DateTime AddedAt, long ProductId, int Quantity) : HistoryPayload;

    public sealed record ChangedHistoryPayload(DateTime AddedAt, long OrderId) : HistoryPayload;

    public sealed record ProcessingHistoryPayload(long OrderId, string Comment) : HistoryPayload;
}