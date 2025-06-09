using System.Runtime.CompilerServices;
using System.Transactions;
using ProductService.Models;
using ProductService.Queries;
using ProductService.Records;
using ProductService.Repositories.Interfaces;
using ProductService.Services.Interfaces;

namespace ProductService.Services.Entities;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderPositionRepository _orderPositionRepository;
    private readonly IHistoryRepository _historyRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderPositionRepository orderPositionRepository,
        IHistoryRepository historyRepository)
    {
        _orderRepository = orderRepository;
        _orderPositionRepository = orderPositionRepository;
        _historyRepository = historyRepository;
    }

    public async Task<RequestResult> CreateOrderAsync(string createdBy, CancellationToken cancellationToken)
    {
        var orderDto = new Order
        {
            OrderState = OrderState.Created,
            CreatedAt = DateTime.Now,
            CreatedBy = createdBy,
        };

        try
        {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);
            orderDto.OrderId = await _orderRepository.CreateOrderAsync(orderDto, cancellationToken);

            var historyDto = new OrderHistoryItem
            {
                OrderId = orderDto.OrderId,
                OrderHistoryCreatedAt = orderDto.CreatedAt,
                OrderHistoryItemKind = OrderHistoryItemKind.Created,
                OrderHistoryItemPayload = new HistoryPayload.CreateHistoryPayload(orderDto.CreatedAt, orderDto.CreatedBy),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);

            transaction.Complete();

            return new RequestResult.Success($"Order created, id : {orderDto.OrderId}", orderDto.OrderId);
        }
        catch (TransactionAbortedException)
        {
            return new RequestResult.Failure("Transaction aborted");
        }
    }

    public async Task<RequestResult> AddProductAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);
            Order order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken);

            if (order.OrderState != OrderState.Created)
            {
                transaction.Complete();
                return new RequestResult.Failure("State is not created");
            }

            var orderPositionDto = new OrderPosition
            {
                OrderId = orderId,
                OrderItemDeleted = false,
                OrderItemQuantity = quantity,
                ProductId = productId,
            };

            await _orderPositionRepository.AddOrderPositionAsync(orderPositionDto, cancellationToken);
            var historyDto = new OrderHistoryItem
            {
                OrderId = orderId,
                OrderHistoryCreatedAt = DateTime.Now,
                OrderHistoryItemKind = OrderHistoryItemKind.ItemAdded,
                OrderHistoryItemPayload = new HistoryPayload.AddedHistoryPayload(DateTime.Now, productId, quantity),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);

            transaction.Complete();
            return new RequestResult.Success("Product added", orderId);
        }
        catch (TransactionAbortedException)
        {
            return new RequestResult.Failure("Transaction aborted");
        }
    }

    public async Task<RequestResult> RemoveProductAsync(long orderId, long productId, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);
            Order order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken);

            if (order.OrderState != OrderState.Created)
            {
                transaction.Complete();
                return new RequestResult.Failure("State is not created");
            }

            await _orderPositionRepository.SoftRemoveOrderPositionAsync(orderId, productId, cancellationToken);

            var historyDto = new OrderHistoryItem
            {
                OrderId = orderId,
                OrderHistoryCreatedAt = DateTime.Now,
                OrderHistoryItemKind = OrderHistoryItemKind.ItemRemoved,
                OrderHistoryItemPayload = new HistoryPayload.RemovedHistoryPayload(DateTime.Now, productId),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);

            transaction.Complete();
            return new RequestResult.Success("Product removed", orderId);
        }
        catch (TransactionAbortedException)
        {
            return new RequestResult.Failure("Transaction aborted");
        }
    }

    public async Task<RequestResult> SetOrderStateProcessingAsync(long orderId, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);

            await _orderRepository.UpdateOrderAsync(orderId, OrderState.Processing, cancellationToken);

            var historyDto = new OrderHistoryItem
            {
                OrderId = orderId,
                OrderHistoryCreatedAt = DateTime.Now,
                OrderHistoryItemKind = OrderHistoryItemKind.StateChanged,
                OrderHistoryItemPayload = new HistoryPayload.ChangedHistoryPayload(DateTime.Now, orderId),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);
            transaction.Complete();
            return new RequestResult.Success("State changed to processing", orderId);
        }
        catch (TransactionAbortedException)
        {
            return new RequestResult.Failure("Transaction aborted");
        }
    }

    public async Task<RequestResult> SaveProcessingOrderStateAsync(long orderId, string comment, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);

            var historyDto = new OrderHistoryItem
            {
                OrderId = orderId,
                OrderHistoryCreatedAt = DateTime.Now,
                OrderHistoryItemKind = OrderHistoryItemKind.StateChanged,
                OrderHistoryItemPayload = new HistoryPayload.ProcessingHistoryPayload(orderId, comment),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);
            transaction.Complete();
            return new RequestResult.Success($"Processing state changed: {comment}", orderId);
        }
        catch (TransactionAbortedException)
        {
            return new RequestResult.Failure("Transaction aborted");
        }
    }

    public async Task<RequestResult> SetOrderStateCompletedAsync(long orderId, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);
            await _orderRepository.UpdateOrderAsync(orderId, OrderState.Completed, cancellationToken);

            var historyDto = new OrderHistoryItem
            {
                OrderId = orderId,
                OrderHistoryCreatedAt = DateTime.Now,
                OrderHistoryItemKind = OrderHistoryItemKind.StateChanged,
                OrderHistoryItemPayload = new HistoryPayload.ChangedHistoryPayload(DateTime.Now, orderId),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);
            transaction.Complete();

            return new RequestResult.Success("State changed to completed", orderId);
        }
        catch (TransactionAbortedException)
        {
            return new RequestResult.Failure("Transaction aborted");
        }
    }

    public async Task<RequestResult> SetOrderStateCancelledAsync(long orderId, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromSeconds(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);

            Order order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken);
            if (order.OrderState != OrderState.Created)
            {
                return new RequestResult.Failure("State is not created");
            }

            await _orderRepository.UpdateOrderAsync(orderId, OrderState.Cancelled, cancellationToken);

            var historyDto = new OrderHistoryItem
            {
                OrderId = orderId,
                OrderHistoryCreatedAt = DateTime.Now,
                OrderHistoryItemKind = OrderHistoryItemKind.ItemRemoved,
                OrderHistoryItemPayload = new HistoryPayload.ChangedHistoryPayload(DateTime.Now, orderId),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);

            transaction.Complete();
            return new RequestResult.Success("State changed to canceled", orderId);
        }
        catch (TransactionAbortedException)
        {
            return new RequestResult.Failure("Transaction aborted");
        }
    }

    public async Task ForceSetOrderStateCancelledAsync(long orderId, CancellationToken cancellationToken)
    {
            using var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromSeconds(5),
                },
                TransactionScopeAsyncFlowOption.Enabled);

            await _orderRepository.UpdateOrderAsync(orderId, OrderState.Cancelled, cancellationToken);

            var historyDto = new OrderHistoryItem
            {
                OrderId = orderId,
                OrderHistoryCreatedAt = DateTime.Now,
                OrderHistoryItemKind = OrderHistoryItemKind.ItemRemoved,
                OrderHistoryItemPayload = new HistoryPayload.ChangedHistoryPayload(DateTime.Now, orderId),
            };

            await _historyRepository.CreateHistoryAsync(historyDto, cancellationToken);

            transaction.Complete();
    }

    public async IAsyncEnumerable<OrderHistory> GetOrderHistoryAsync(
        long orderId,
        int limit,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        long cursor = 0;
        long previous;
        do
        {
            IAsyncEnumerable<OrderHistoryItem> history = _historyRepository.QueryHistoryAsync(
                new QueryHistoryParameters(cursor, limit, [orderId], null),
                cancellationToken);
            previous = cursor;
            await foreach (OrderHistoryItem historyItemDto in history)
            {
                cursor = historyItemDto.OrderHistoryItemId;
                if (cursor == previous)
                    break;
                yield return new OrderHistory(
                    historyItemDto.OrderId,
                    historyItemDto.OrderHistoryCreatedAt,
                    historyItemDto.OrderHistoryItemKind,
                    historyItemDto.OrderHistoryItemPayload);
            }
        }
        while (cursor != previous);
    }
}