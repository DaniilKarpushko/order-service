namespace ProductService.Queries;

public record QueryOrderPositionParameters(long Cursor, int Limit, long[] ProductIds, long[] OrderIds, bool? OrderItemDeleted);