using System.Collections.Concurrent;
using DddBookstore.Before;

namespace DddBookstore.Web;

/// <summary>
/// WITHOUT DDD: the order is plain data with public setters, and the rules are split
/// between Before.OrderService and this class. Whatever OrderService doesn't cover
/// has to be written here, in the UI layer.
/// </summary>
public sealed class BeforeOrderFacade : IOrderFacade
{
    private readonly OrderService _service = new();
    private readonly ConcurrentDictionary<Guid, OrderData> _orders = new();

    public OrderResponse Create(decimal unitPrice, int quantity)
    {
        // OrderService only accepts paid orders, so isPaid: true is passed just to reuse its validation.
        var result = _service.PlaceOrder(unitPrice, quantity, isPaid: true);
        if (!result.Success)
            return OrderResponse.Fail(result.Message);

        var order = new OrderData { Id = Guid.NewGuid(), UnitPrice = unitPrice, Quantity = quantity, Total = result.Total };
        _orders[order.Id] = order;
        return OrderResponse.Ok(Messages.Created, ToView(order));
    }

    public OrderResponse Pay(Guid orderId)
    {
        if (!_orders.TryGetValue(orderId, out var order))
            return OrderResponse.Fail(Messages.NotFound);

        // Not in OrderService, so the rule lives here.
        if (order.IsPaid || order.IsCancelled)
            return OrderResponse.Fail("فقط سفارش در انتظار پرداخت قابل پرداخت است.");

        order.IsPaid = true;
        return OrderResponse.Ok(Messages.Paid, ToView(order));
    }

    public OrderResponse Ship(Guid orderId)
    {
        if (!_orders.TryGetValue(orderId, out var order))
            return OrderResponse.Fail(Messages.NotFound);

        // OrderService.ShipOrder doesn't know about cancellation.
        if (order.IsCancelled)
            return OrderResponse.Fail("سفارش لغوشده قابل ارسال نیست.");

        var result = _service.ShipOrder(order.IsPaid, order.IsShipped);
        if (!result.Success)
            return OrderResponse.Fail(result.Message);

        order.IsShipped = true;
        return OrderResponse.Ok(result.Message, ToView(order));
    }

    public OrderResponse Cancel(Guid orderId)
    {
        if (!_orders.TryGetValue(orderId, out var order))
            return OrderResponse.Fail(Messages.NotFound);

        // Not in OrderService either.
        if (order.IsShipped)
            return OrderResponse.Fail("سفارش ارسال‌شده قابل لغو نیست.");

        if (order.IsCancelled)
            return OrderResponse.Fail("سفارش قبلاً لغو شده است.");

        order.IsCancelled = true;
        return OrderResponse.Ok(Messages.Cancelled, ToView(order));
    }

    private static OrderView ToView(OrderData order)
    {
        var status = order.IsCancelled ? "Cancelled"
            : order.IsShipped ? "Shipped"
            : order.IsPaid ? "Paid"
            : "PendingPayment";

        return new OrderView(order.Id, order.UnitPrice, order.Quantity, order.Total, status);
    }

    private sealed class OrderData
    {
        public Guid Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public bool IsPaid { get; set; }
        public bool IsShipped { get; set; }
        public bool IsCancelled { get; set; }
    }
}
