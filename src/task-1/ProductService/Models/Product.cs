namespace ProductService.Models;

public class Product
{
    public long ProductId { get; set; }

    public required string ProductName { get; init; }

    public decimal ProductPrice { get; init; }
}