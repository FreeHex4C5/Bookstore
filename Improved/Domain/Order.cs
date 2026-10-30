namespace DddBookstore.Improved.Domain;

public enum OrderStatus
{
    PendingPayment,
    Paid,
    Shipped,
    Cancelled
}

/// <summary>
/// Aggregate Root: the only way to change an order is through its behaviours.
/// Price and quantity are already valid Value Objects, so creation needs no checks here.
/// </summary>
public sealed class Order
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public OrderId Id { get; }
    public Price UnitPrice { get; }
    public Quantity Quantity { get; }
    public decimal Total => UnitPrice.Value * Quantity.Value;
    public OrderStatus Status { get; private set; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    private Order(OrderId id, Price unitPrice, Quantity quantity)
    {
        Id = id;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Status = OrderStatus.PendingPayment;
    }

    public static Order Create(Price unitPrice, Quantity quantity)
    {
        var order = new Order(OrderId.New(), unitPrice, quantity);
        order._domainEvents.Add(new OrderCreated(order.Id, order.Total));
        return order;
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new DomainException("فقط سفارش در انتظار پرداخت قابل پرداخت است.");

        Status = OrderStatus.Paid;
        _domainEvents.Add(new OrderPaid(Id));
    }

    public void Ship()
    {
        if (Status == OrderStatus.Shipped)
            throw new DomainException("سفارش قبلاً ارسال شده است.");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("سفارش لغوشده قابل ارسال نیست.");

        if (Status != OrderStatus.Paid)
            throw new DomainException("سفارش پرداخت نشده؛ ارسال ممکن نیست.");

        Status = OrderStatus.Shipped;
        _domainEvents.Add(new OrderShipped(Id));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new DomainException("سفارش ارسال‌شده قابل لغو نیست.");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("سفارش قبلاً لغو شده است.");

        Status = OrderStatus.Cancelled;
        _domainEvents.Add(new OrderCancelled(Id));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
