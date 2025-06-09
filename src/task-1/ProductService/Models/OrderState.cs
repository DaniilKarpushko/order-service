using NpgsqlTypes;

namespace ProductService.Models;

public enum OrderState
{
    [PgName("created")]
    Created,
    [PgName("processing")]
    Processing,
    [PgName("completed")]
    Completed,
    [PgName("cancelled")]
    Cancelled,
}