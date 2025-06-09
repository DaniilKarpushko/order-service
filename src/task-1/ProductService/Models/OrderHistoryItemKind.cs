using NpgsqlTypes;

namespace ProductService.Models;

public enum OrderHistoryItemKind
{
    [PgName("unspecified")]
    Unspecified,
    [PgName("created")]
    Created,
    [PgName("item_added")]
    ItemAdded,
    [PgName("item_removed")]
    ItemRemoved,
    [PgName("state_changed")]
    StateChanged,
}