using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Npgsql;
using ProductService.Models;
using ProductService.Queries;
using ProductService.Repositories.Interfaces;

namespace ProductService.Repositories.Entities;

public class HistoryRepository : IHistoryRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public HistoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task CreateHistoryAsync(OrderHistoryItem item, CancellationToken cancellationToken)
    {
        const string sql = """
                     insert into order_history (order_id, order_history_item_created_at, order_history_item_kind, order_history_item_payload)
                     values (:order_id, :order_history_created_at, :order_history_item_kind, :order_history_item_payload::jsonb);
                     """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("order_id", item.OrderId),
                new NpgsqlParameter("order_history_created_at", item.OrderHistoryCreatedAt),
                new NpgsqlParameter("order_history_item_kind", item.OrderHistoryItemKind),
                new NpgsqlParameter(
                    "order_history_item_payload",
                    JsonSerializer.Serialize(item.OrderHistoryItemPayload)),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<OrderHistoryItem> QueryHistoryAsync(
        QueryHistoryParameters parameters,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string sql = """
                     select order_history_item_id, order_id, order_history_item_created_at, order_history_item_kind, order_history_item_payload
                     from order_history
                     where order_history_item_id >= :cursor
                     and (:order_history_item_kind::order_history_item_kind is null or order_history_item_kind = :order_history_item_kind)
                     and (array_length(:order_history_item_ids, 1) = 0 or order_id = any(:order_history_item_ids))
                     limit :limit
                     """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", parameters.Cursor),
                new NpgsqlParameter("order_history_item_kind", parameters.OrderHistoryItemKind is null ? DBNull.Value : parameters.OrderHistoryItemKind),
                new NpgsqlParameter("order_history_item_ids", parameters.OrderHistoryItemIds),
                new NpgsqlParameter("limit", parameters.Limit),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new OrderHistoryItem
            {
                OrderHistoryItemId = reader.GetInt64("order_history_item_id"),
                OrderId = reader.GetInt64("order_id"),
                OrderHistoryCreatedAt = reader.GetDateTime("order_history_item_created_at"),
                OrderHistoryItemKind = reader.GetFieldValue<OrderHistoryItemKind>("order_history_item_kind"),
                OrderHistoryItemPayload = JsonSerializer.Deserialize<HistoryPayload>(reader.GetFieldValue<string>("order_history_item_payload")),
            };
        }
    }
}