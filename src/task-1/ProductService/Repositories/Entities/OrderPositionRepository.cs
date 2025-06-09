using System.Data;
using System.Runtime.CompilerServices;
using Npgsql;
using ProductService.Models;
using ProductService.Queries;
using ProductService.Repositories.Interfaces;

namespace ProductService.Repositories.Entities;

public class OrderPositionRepository : IOrderPositionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderPositionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddOrderPositionAsync(OrderPosition orderPosition, CancellationToken cancellationToken)
    {
        const string sql = """
                     insert into order_items (order_id, product_id, order_item_quantity, order_item_deleted )
                     values (:order_id, :product_id, :order_item_quantity, :order_item_deleted )
                     """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("order_id", orderPosition.OrderId),
                new NpgsqlParameter("product_id", orderPosition.ProductId),
                new NpgsqlParameter("order_item_quantity", orderPosition.OrderItemQuantity),
                new NpgsqlParameter("order_item_deleted", orderPosition.OrderItemDeleted),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SoftRemoveOrderPositionAsync(long id, long productId, CancellationToken cancellationToken)
    {
        string sql = """
                     update order_items
                     set order_item_deleted = true
                     where order_id = :order_id
                     and product_id = :product_id
                     """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("order_id", id),
                new NpgsqlParameter("product_id", productId),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<OrderPosition> QueryOrderPositionAsync(
        QueryOrderPositionParameters parameters,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string sql = """
                     select order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
                     from order_items
                     where order_item_id > :cursor
                     and ( cardinality(:order_id ) = 0 or order_id = any(:order_id))
                     and ( cardinality(:product_id) = 0 or product_id = any(:product_id))
                     and (order_item_deleted is null or order_item_deleted = :order_item_deleted)
                     order by order_item_id
                     limit :limit
                     """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", parameters.Cursor),
                new NpgsqlParameter("order_id", parameters.OrderIds),
                new NpgsqlParameter("product_id", parameters.ProductIds),
                new NpgsqlParameter("order_item_deleted", parameters.OrderItemDeleted is null ? DBNull.Value : parameters.OrderItemDeleted),
                new NpgsqlParameter("limit", parameters.Limit),
            },
        };

        while (await command.ExecuteReaderAsync(cancellationToken) is { } reader)
        {
            yield return new OrderPosition
            {
                OrderId = reader.GetInt64("order_id"),
                ProductId = reader.GetInt64("product_id"),
                OrderItemQuantity = reader.GetInt32("order_item_quantity"),
                OrderItemDeleted = reader.GetBoolean("order_item_deleted"),
            };
        }
    }
}