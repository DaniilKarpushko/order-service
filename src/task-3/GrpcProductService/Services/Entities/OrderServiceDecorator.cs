using Google.Protobuf.WellKnownTypes;
using Kafka.Options;
using Kafka.Producer;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using ProductService.Records;
using ProductService.Repositories.Interfaces;
using ProductService.Services.Entities;
using ProductService.Services.Interfaces;

namespace GrpcProductService.Services.Entities;

public class OrderServiceDecorator : IOrderService
{
    private readonly OrderService _orderService;
    private readonly IKafkaProducer<OrderCreationKey, OrderCreationValue> _producer;

    private readonly string _topic;

    public OrderServiceDecorator(
        IKafkaProducer<OrderCreationKey, OrderCreationValue> producer,
        IOrderRepository orderRepository,
        IOrderPositionRepository orderPositionRepository,
        IHistoryRepository historyRepository,
        IOptions<KafkaProducerOptions> kafkaProducerOptions)
    {
        _orderService = new OrderService(orderRepository, orderPositionRepository, historyRepository);
        _producer = producer;

        _topic = kafkaProducerOptions.Value.Topic;
    }

    public async Task<RequestResult> CreateOrderAsync(string createdBy, CancellationToken cancellationToken)
    {
        RequestResult result = await _orderService.CreateOrderAsync(createdBy, cancellationToken);

        if (result is RequestResult.Success success)
        {
            await _producer.SendMessageAsync(
                _topic,
                new OrderCreationKey { OrderId = success.Id },
                new OrderCreationValue
                {
                    OrderCreated = new OrderCreationValue.Types.OrderCreated
                    {
                        CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                        OrderId = success.Id,
                    },
                },
                cancellationToken);
        }

        return result;
    }

    public async Task<RequestResult> AddProductAsync(
        long orderId,
        long productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        return await _orderService.AddProductAsync(orderId, productId, quantity, cancellationToken);
    }

    public async Task<RequestResult> RemoveProductAsync(
        long orderId,
        long productId,
        CancellationToken cancellationToken)
    {
        return await _orderService.RemoveProductAsync(orderId, productId, cancellationToken);
    }

    public async Task<RequestResult> SetOrderStateProcessingAsync(long orderId, CancellationToken cancellationToken)
    {
        RequestResult result = await _orderService.SetOrderStateProcessingAsync(orderId, cancellationToken);

        if (result is RequestResult.Success success)
        {
            await _producer.SendMessageAsync(
                _topic,
                new OrderCreationKey { OrderId = orderId },
                new OrderCreationValue()
                {
                    OrderProcessingStarted = new OrderCreationValue.Types.OrderProcessingStarted
                    {
                        OrderId = success.Id,
                        StartedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                    },
                },
                cancellationToken);
        }

        return result;
    }

    public async Task<RequestResult> SaveProcessingOrderStateAsync(
        long orderId,
        string comment,
        CancellationToken cancellationToken)
    {
        return await _orderService.SaveProcessingOrderStateAsync(orderId, comment, cancellationToken);
    }

    public async Task<RequestResult> SetOrderStateCompletedAsync(long orderId, CancellationToken cancellationToken)
    {
        return await _orderService.SetOrderStateCompletedAsync(orderId, cancellationToken);
    }

    public async Task<RequestResult> SetOrderStateCancelledAsync(long orderId, CancellationToken cancellationToken)
    {
        return await _orderService.SetOrderStateCancelledAsync(orderId, cancellationToken);
    }

    public async Task ForceSetOrderStateCancelledAsync(long orderId, CancellationToken cancellationToken)
    {
        await _orderService.ForceSetOrderStateCancelledAsync(orderId, cancellationToken);
    }

    public IAsyncEnumerable<OrderHistory> GetOrderHistoryAsync(
        long orderId,
        int limit,
        CancellationToken cancellationToken)
    {
        return _orderService.GetOrderHistoryAsync(orderId, limit, cancellationToken);
    }
}