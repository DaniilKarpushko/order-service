using ProductService.Models;

namespace ProductService.Queries;

public record QueryHistoryParameters(long Cursor, int Limit, long[] OrderHistoryItemIds, OrderHistoryItemKind? OrderHistoryItemKind);