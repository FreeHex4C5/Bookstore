namespace DddBookstore.After.Domain;

public enum OrderStatus
{
    PendingPayment,
    Paid,
    Shipped,
    Cancelled
}

/// <summary>
/// Entity: owns the order's state and protects rules for changing it.
/// </summary>
public sealed class Order
{
    public Guid Id { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }
    public decimal Total => UnitPrice * Quantity;
    public OrderStatus Status { get; private set; }

    private Order(decimal unitPrice, int quantity)
    {
        Id = Guid.NewGuid();
        UnitPrice = unitPrice;
        Quantity = quantity;
        Status = OrderStatus.PendingPayment;
    }

    public static Order Create(decimal unitPrice, int quantity)
    {
        if (unitPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "قیمت باید بیشتر از صفر باشد.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "تعداد باید بیشتر از صفر باشد.");

        return new Order(unitPrice, quantity);
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new InvalidOperationException("فقط سفارش در انتظار پرداخت قابل پرداخت است.");

        Status = OrderStatus.Paid;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException("فقط سفارش پرداخت‌شده قابل ارسال است.");

        Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("سفارش ارسال‌شده قابل لغو نیست.");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("سفارش قبلاً لغو شده است.");

        Status = OrderStatus.Cancelled;
    }
}
