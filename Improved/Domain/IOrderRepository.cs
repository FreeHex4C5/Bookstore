namespace DddBookstore.Improved.Domain;

/// <summary>
/// Repository: a collection-like abstraction for loading and saving whole aggregates.
/// The domain defines it; infrastructure implements it.
/// </summary>
public interface IOrderRepository
{
    Order? GetById(OrderId id);

    void Save(Order order);
}
