namespace ProductService.Models;

public class OrderPosition
{
    public long OrderItemId { get; set; }

    public long OrderId { get; init; }

    public long ProductId { get; init; }

    public int OrderItemQuantity { get; init; }

    public bool OrderItemDeleted { get; init; }
}