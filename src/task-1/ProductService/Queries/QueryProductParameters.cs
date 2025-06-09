namespace ProductService.Queries;

public record QueryProductParameters(long Cursor,  int Limit, long[] Ids, int? MinPrice, int? MaxPrice, string? NamePattern);