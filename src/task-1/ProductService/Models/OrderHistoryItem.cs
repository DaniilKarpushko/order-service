namespace ProductService.Models;

public class OrderHistoryItem
{
    public long OrderHistoryItemId { get; init; }

    public long OrderId { get; init; }

    public DateTime OrderHistoryCreatedAt { get; init; }

    public OrderHistoryItemKind OrderHistoryItemKind { get; init; }

    public required HistoryPayload? OrderHistoryItemPayload { get; init; }
}