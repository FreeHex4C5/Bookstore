using DddBookstore.Improved.Domain;

namespace DddBookstore.Improved.Application;

/// <summary>
/// Coordinates each use case: load the aggregate, call its behaviour, save it, publish its events.
/// It contains no business rules and returns DTOs instead of exposing the entity.
/// </summary>
public sealed class OrderAppService
{
    private readonly IOrderRepository _repository;
    private readonly IDomainEventPublisher _events;

    public OrderAppService(IOrderRepository repository, IDomainEventPublisher events)
    {
        _repository = repository;
        _events = events;
    }

    public OrderDto Create(decimal unitPrice, int quantity)
    {
        // Quantity is validated before price, the same order as the Before version.
        var validQuantity = Quantity.Of(quantity);
        var validPrice = Price.Of(unitPrice);

        return Save(Order.Create(validPrice, validQuantity));
    }

    public OrderDto Pay(Guid orderId) => Change(orderId, order => order.MarkAsPaid());

    public OrderDto Ship(Guid orderId) => Change(orderId, order => order.Ship());

    public OrderDto Cancel(Guid orderId) => Change(orderId, order => order.Cancel());

    private OrderDto Change(Guid orderId, Action<Order> behaviour)
    {
        var order = _repository.GetById(new OrderId(orderId)) ?? throw new OrderNotFoundException(orderId);
        behaviour(order);
        return Save(order);
    }

    private OrderDto Save(Order order)
    {
        _repository.Save(order);

        foreach (var domainEvent in order.DomainEvents)
            _events.Publish(domainEvent);

        order.ClearDomainEvents();
        return OrderDto.From(order);
    }
}

public sealed record OrderDto(Guid Id, decimal UnitPrice, int Quantity, decimal Total, string Status)
{
    public static OrderDto From(Order order) =>
        new(order.Id.Value, order.UnitPrice.Value, order.Quantity.Value, order.Total, order.Status.ToString());
}

public interface IDomainEventPublisher
{
    void Publish(IDomainEvent domainEvent);
}

public sealed class OrderNotFoundException : Exception
{
    public OrderNotFoundException(Guid orderId) : base("سفارش پیدا نشد.")
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
