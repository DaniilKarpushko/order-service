using System.Data;
using System.Runtime.CompilerServices;
using Npgsql;
using ProductService.Models;
using ProductService.Queries;
using ProductService.Repositories.Interfaces;

namespace ProductService.Repositories.Entities;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        string sql = """
                     insert into orders (order_state, order_created_at,  order_created_by)
                     values (:state, :created_at, :created_by)
                     returning order_id
                     """;
        await using NpgsqlConnection
            connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("state", order.OrderState),
                new NpgsqlParameter("created_at", order.CreatedAt),
                new NpgsqlParameter("created_by", order.CreatedBy),
            },
        };

        long orderId = (long?)await command.ExecuteScalarAsync(cancellationToken) ?? -1;

        return orderId;
    }

    public async Task UpdateOrderAsync(long id, OrderState state, CancellationToken cancellationToken)
    {
        string sql = """
                     update orders
                     set order_state = :state
                     where order_id = :order_id
                     """;

        await using NpgsqlConnection
            connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("state", state),
                new NpgsqlParameter("order_id", id),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Order> GetOrderByIdAsync(long id, CancellationToken cancellationToken)
    {
        string sql = """
                     select order_id, order_state, order_created_at, order_created_by
                     from orders
                     where order_id = :order_id
                     """;
        await using NpgsqlConnection
            connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("order_id", id),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new Order
            {
                OrderId = reader.GetInt64("order_id"),
                OrderState = reader.GetFieldValue<OrderState>("order_state"),
                CreatedAt = reader.GetDateTime("order_created_at"),
                CreatedBy = reader.GetString("order_created_by"),
            };
        }

        return new Order();
    }

    public async IAsyncEnumerable<Order> QueryOrderAsync(
        QueryOrderParameters orderParameters,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string sql = """
                     select order_id, order_state, order_created_at, order_created_by
                     from orders
                     where order_id >= :cursor
                     and( :state::text is null or order_state = :state )
                     and( order_created_by is null or order_created_by = :created_by )
                     and (order_created_at is null or order_created_at = :created_at)
                     order by order_id
                     limit :limit
                     """;

        await using NpgsqlConnection
            connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", orderParameters.Cursor),
                new NpgsqlParameter("state", orderParameters.State),
                new NpgsqlParameter("created_by", orderParameters.CreatedBy),
                new NpgsqlParameter("created_at", orderParameters.CreatedAt),
                new NpgsqlParameter("limit", orderParameters.Limit),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Order
            {
                OrderId = reader.GetInt64("order_id"),
                OrderState = reader.GetFieldValue<OrderState>("order_state"),
                CreatedAt = reader.GetDateTime("order_created_at"),
                CreatedBy = reader.GetString("order_created_by"),
            };
        }
    }
}