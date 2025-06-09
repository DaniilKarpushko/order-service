using GrpcProductService.Protos;

namespace GrpcProductService.ExtensionServices;

public static class EnumExtensions
{
    public static ProductService.Models.OrderHistoryItemKind ToOrderHistoryItemKind(this OrderHistoryItemKind kind)
    {
        return kind switch
        {
            OrderHistoryItemKind.Created => ProductService.Models.OrderHistoryItemKind.Created,
            OrderHistoryItemKind.ItemAdded => ProductService.Models.OrderHistoryItemKind.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => ProductService.Models.OrderHistoryItemKind.ItemRemoved,
            OrderHistoryItemKind.StateChanged => ProductService.Models.OrderHistoryItemKind.StateChanged,
            OrderHistoryItemKind.Unspecified => ProductService.Models.OrderHistoryItemKind.Unspecified,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
    }

    public static OrderHistoryItemKind ToGrpcOrderHistoryItemKind(this ProductService.Models.OrderHistoryItemKind kind)
    {
        return kind switch
        {
            ProductService.Models.OrderHistoryItemKind.Created => OrderHistoryItemKind.Created,
            ProductService.Models.OrderHistoryItemKind.ItemAdded => OrderHistoryItemKind.ItemAdded,
            ProductService.Models.OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
            ProductService.Models.OrderHistoryItemKind.StateChanged => OrderHistoryItemKind.StateChanged,
            ProductService.Models.OrderHistoryItemKind.Unspecified => OrderHistoryItemKind.Unspecified,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
    }
}