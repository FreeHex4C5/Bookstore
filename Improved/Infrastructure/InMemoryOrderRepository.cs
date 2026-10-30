using System.Collections.Concurrent;
using DddBookstore.Improved.Domain;

namespace DddBookstore.Improved.Infrastructure;

/// <summary>
/// Teaching-only storage. A real project would use a database here, without changing the domain.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<OrderId, Order> _orders = new();

    public Order? GetById(OrderId id) => _orders.GetValueOrDefault(id);

    public void Save(Order order) => _orders[order.Id] = order;
}
