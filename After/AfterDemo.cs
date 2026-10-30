using DddBookstore.After.Application;
using DddBookstore.After.Domain;

namespace DddBookstore.After;

public static class AfterDemo
{
    public static void Run()
    {
        Console.WriteLine("=== AFTER: مدل دامنه وضعیت و قانون‌های سفارش را کنترل می‌کند ===");
        var app = new OrderAppService();

        Try("ساخت سفارش با تعداد صفر", () => app.Create(20m, 0));

        var order = app.Create(20m, 2);
        Console.WriteLine($"سفارش ساخته شد. مبلغ: {order.Total} | وضعیت: {order.Status}");

        Try("ارسال پیش از پرداخت", () => app.Ship(order));

        app.Pay(order);
        Console.WriteLine($"پس از پرداخت: {order.Status}");

        app.Ship(order);
        Console.WriteLine($"پس از ارسال: {order.Status}");

        Try("لغو سفارش ارسال‌شده", () => app.Cancel(order));

        Console.WriteLine();
        Console.WriteLine("مدل Order مالک وضعیت و رفتار سفارش است؛ لایه Application فقط عملیات را هماهنگ می‌کند.");
        Console.WriteLine("این پروژه آموزشی است و برای ساده‌ماندن، API و دیتابیس ندارد.");
    }

    private static void Try(string title, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"{title}: موفق");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{title}: ناموفق — {ex.Message}");
        }
    }
}
