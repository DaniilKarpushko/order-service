using ProductService.Models;

namespace ProductService.Records;

public record OrderHistory(long OrderId, DateTime OrderHistoryCreatedAt, OrderHistoryItemKind OrderHistoryItemKind, HistoryPayload? OrderHistoryItemPayload);