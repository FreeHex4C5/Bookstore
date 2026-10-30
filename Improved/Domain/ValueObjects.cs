namespace DddBookstore.Improved.Domain;

/// <summary>
/// Value Object: a book's unit price. An invalid price can never be created.
/// </summary>
public sealed record Price
{
    public decimal Value { get; }

    private Price(decimal value) => Value = value;

    public static Price Of(decimal value) =>
        value > 0 ? new Price(value) : throw new DomainException("قیمت کتاب باید بیشتر از صفر باشد.");
}

/// <summary>
/// Value Object: number of books in an order. An invalid quantity can never be created.
/// </summary>
public sealed record Quantity
{
    public int Value { get; }

    private Quantity(int value) => Value = value;

    public static Quantity Of(int value) =>
        value > 0 ? new Quantity(value) : throw new DomainException("تعداد کتاب باید بیشتر از صفر باشد.");
}

/// <summary>
/// Strongly-typed identity, so an order id can't be confused with any other Guid.
/// </summary>
public readonly record struct OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
