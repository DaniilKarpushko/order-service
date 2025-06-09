using System.Data;
using System.Runtime.CompilerServices;
using Npgsql;
using ProductService.Models;
using ProductService.Queries;
using ProductService.Repositories.Interfaces;

namespace ProductService.Repositories.Entities;

public class ProductsRepository : IProductsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProductsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        string sql = """
                     insert into products (product_name, product_price)
                     values (:name, :price)
                     returning product_id
                     """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("name", product.ProductName),
                new NpgsqlParameter("price", product.ProductPrice),
            },
        };

        long productId = (long?)await command.ExecuteScalarAsync(cancellationToken) ?? -1;

        return productId;
    }

    public async IAsyncEnumerable<Product> QueryProductAsync(
        QueryProductParameters parameters,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string sql = """
                     select product_id, product_name, product_price
                     from products
                     where id >= :cursor
                     and( cardinality(:ids) = 0 or product_id = any(:ids) )
                     and( :name_pattern is null or product_name like :name_pattern )
                     and ( :min_price is null or product_price >= :min_price )
                     and ( :max_price is null or product_price <= :max_price )
                     order by id
                     limit :limit
                     """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", parameters.Cursor),
                new NpgsqlParameter("name_pattern", parameters.NamePattern),
                new NpgsqlParameter("ids", parameters.Ids),
                new NpgsqlParameter("min_price", parameters.MinPrice),
                new NpgsqlParameter("max_price", parameters.MaxPrice),
            },
        };

        while (await command.ExecuteReaderAsync(cancellationToken) is { } reader)
        {
            yield return new Product
            {
                ProductId = reader.GetInt64("product_id"),
                ProductName = reader.GetString("product_name"),
                ProductPrice = reader.GetDecimal("product_price"),
            };
        }
    }
}