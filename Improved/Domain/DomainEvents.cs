namespace DddBookstore.Improved.Domain;

/// <summary>
/// Something meaningful that happened in the domain (named in the past tense).
/// </summary>
public interface IDomainEvent
{
}

public sealed record OrderCreated(OrderId OrderId, decimal Total) : IDomainEvent;

public sealed record OrderPaid(OrderId OrderId) : IDomainEvent;

public sealed record OrderShipped(OrderId OrderId) : IDomainEvent;

public sealed record OrderCancelled(OrderId OrderId) : IDomainEvent;
