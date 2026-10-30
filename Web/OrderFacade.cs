namespace DddBookstore.Web;

/// <summary>
/// What the UI needs. Both versions implement it, so the UI can't tell them apart.
/// </summary>
public interface IOrderFacade
{
    OrderResponse Create(decimal unitPrice, int quantity);
    OrderResponse Pay(Guid orderId);
    OrderResponse Ship(Guid orderId);
    OrderResponse Cancel(Guid orderId);
}

public sealed record OrderView(Guid Id, decimal UnitPrice, int Quantity, decimal Total, string Status);

public sealed record OrderResponse(bool Success, string Message, OrderView? Order)
{
    public static OrderResponse Ok(string message, OrderView order) => new(true, message, order);

    public static OrderResponse Fail(string message) => new(false, message, null);
}

/// <summary>
/// Success messages shown by the UI, shared so both versions say exactly the same thing.
/// </summary>
public static class Messages
{
    public const string Created = "سفارش ثبت شد.";
    public const string Paid = "پرداخت انجام شد.";
    public const string Shipped = "سفارش برای ارسال آماده شد.";
    public const string Cancelled = "سفارش لغو شد.";
    public const string NotFound = "سفارش پیدا نشد.";
}
