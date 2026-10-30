namespace DddBookstore.Before;

// BEFORE: the order is just data, and business rules live in a service.
public sealed class OrderService
{
    public OrderResult PlaceOrder(decimal unitPrice, int quantity, bool isPaid)
    {
        if (quantity <= 0)
            return new OrderResult(false, "تعداد کتاب باید بیشتر از صفر باشد.", 0);

        if (unitPrice <= 0)
            return new OrderResult(false, "قیمت کتاب باید بیشتر از صفر باشد.", 0);

        if (!isPaid)
            return new OrderResult(false, "سفارش پرداخت نشده است.", 0);

        return new OrderResult(true, "سفارش ثبت شد.", unitPrice * quantity);
    }

    public OrderResult ShipOrder(bool isPaid, bool alreadyShipped)
    {
        if (!isPaid)
            return new OrderResult(false, "سفارش پرداخت نشده؛ ارسال ممکن نیست.", 0);

        if (alreadyShipped)
            return new OrderResult(false, "سفارش قبلاً ارسال شده است.", 0);

        return new OrderResult(true, "سفارش برای ارسال آماده شد.", 0);
    }
}

public sealed record OrderResult(bool Success, string Message, decimal Total);

public static class BeforeDemo
{
    public static void Run()
    {
        Console.WriteLine("=== BEFORE: قانون‌ها داخل سرویس و داده‌ها جدا از رفتار هستند ===");
        var service = new OrderService();

        Print("تعداد صفر", service.PlaceOrder(20m, 0, true));
        Print("سفارش پرداخت‌نشده", service.PlaceOrder(20m, 2, false));
        Print("سفارش معتبر", service.PlaceOrder(20m, 2, true));

        Console.WriteLine();
        Console.WriteLine("این کد لزوماً بد نیست؛ مشکل وقتی شروع می‌شود که قانون‌ها در مسیرهای مختلف تکرار شوند.");
        Console.WriteLine("همچنین اگر داده‌های سفارش جای دیگری مستقیم تغییر کنند، ممکن است قانون ارسال دور زده شود.");
    }

    private static void Print(string title, OrderResult result) =>
        Console.WriteLine($"{title}: {(result.Success ? "موفق" : "ناموفق")} — {result.Message}" +
                          (result.Total > 0 ? $" مبلغ: {result.Total}" : ""));
}
