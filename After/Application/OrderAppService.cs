using DddBookstore.After.Domain;

namespace DddBookstore.After.Application;

/// <summary>
/// Coordinates the use case. Business invariants remain in the domain entity.
/// </summary>
public sealed class OrderAppService
{
    public Order Create(decimal unitPrice, int quantity) =>
        Order.Create(unitPrice, quantity);

    public void Pay(Order order) => order.MarkAsPaid();

    public void Ship(Order order) => order.Ship();

    public void Cancel(Order order) => order.Cancel();
}
