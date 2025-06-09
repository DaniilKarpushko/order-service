namespace ProductService.Models;

public class Order
{
    public long OrderId { get; set; }

    public OrderState OrderState { get; init; }

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = string.Empty;
}