using ProductService.Models;

namespace ProductService.Queries;

public record QueryOrderParameters(long Cursor, int Limit, OrderState? State, string? CreatedBy, DateTime? CreatedAt);