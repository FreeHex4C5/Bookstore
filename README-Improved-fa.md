# نسخه‌ی بهبودیافته‌ی DDD و رابط کاربری مقایسه

این بخش **به کدهای ویدیو (`Before/`، `After/`، `Program.cs`) دست نمی‌زند**؛ فقط فایل جدید اضافه شده است.
دستورهای `dotnet run -- --before` و `dotnet run` دقیقاً مثل قبل کار می‌کنند.

## اجرای رابط کاربری
```bash
dotnet run --project Web
```
سپس آدرس <http://localhost:5080> را باز کنید.

- ستون راست: نسخه‌ی **بدون DDD** (همان `OrderService` در `Before/BeforeDemo.cs`)
- ستون چپ: نسخه‌ی **با DDD** (پوشه‌ی `Improved/`)
- دکمه‌ی «اجرای سناریوی ویدیو روی هر دو» همان مراحل `AfterDemo` را روی هر دو اجرا می‌کند و در پایان می‌گوید خروجی‌ها یکسان است یا نه.

پیام اصلی: **کاربر هیچ فرقی نمی‌بیند.** DDD درباره‌ی شکل درونی کد است، نه خروجی. فرق در جایی است که قانون‌ها نوشته شده‌اند.

## چه چیزی نسبت به `After/` بهتر شده؟ (پوشه‌ی `Improved/`)

| مفهوم | فایل | توضیح |
|---|---|---|
| Value Object | `Domain/ValueObjects.cs` | `Price` و `Quantity` خودشان معتبر بودن را تضمین می‌کنند؛ مقدار نامعتبر اصلاً ساخته نمی‌شود. |
| شناسه‌ی نوع‌دار | `Domain/ValueObjects.cs` | `OrderId` به‌جای `Guid` خام؛ با شناسه‌ی چیز دیگری قاطی نمی‌شود. |
| Aggregate Root | `Domain/Order.cs` | تنها راه تغییر سفارش رفتارهای خودش است (`MarkAsPaid`، `Ship`، `Cancel`). قانون «سفارش لغوشده قابل ارسال نیست» هم اضافه شده. |
| Domain Exception | `Domain/DomainException.cs` | خطای قانون کسب‌وکار از خطای فنی جدا شده است. |
| Domain Event | `Domain/DomainEvents.cs` | `OrderCreated`، `OrderPaid`، … ؛ در کنسول سرور چاپ می‌شوند (جایی که بعداً می‌شود ایمیل یا انبار را خبر کرد). |
| Repository | `Domain/IOrderRepository.cs` + `Infrastructure/InMemoryOrderRepository.cs` | دامنه فقط قرارداد را می‌شناسد؛ پیاده‌سازی (حافظه/دیتابیس) در Infrastructure است. |
| Application Service | `Application/OrderAppService.cs` | با `Id` کار می‌کند: بارگذاری ← اجرای رفتار ← ذخیره ← انتشار رویداد. به‌جای خود Entity، `OrderDto` برمی‌گرداند. |

## نکته‌ی آموزشی از پوشه‌ی `Web/`
- `Web/BeforeOrderFacade.cs`: چون `OrderService` قانون پرداخت و لغو را ندارد، این قانون‌ها مجبوراً **در لایه‌ی UI** نوشته شده‌اند؛ برای ثبت سفارش هم باید `isPaid: true` فرستاد تا اعتبارسنجی اجرا شود. این همان «پخش‌شدن قانون‌ها» است.
- `Web/DddOrderFacade.cs`: **هیچ قانونی ندارد**؛ فقط Application Service را صدا می‌زند.

هر دو یک رابط (`IOrderFacade`) را پیاده می‌کنند و یک صفحه‌ی HTML با یک استایل برای هر دو استفاده می‌شود.

## یادداشت فنی
فایل `Directory.Build.props` فقط برای این است که پروژه‌ی کنسولی اصلی، فایل‌های پوشه‌ی `Web/` را کامپایل نکند (تا لازم نباشد `.csproj` اصلی تغییر کند).
