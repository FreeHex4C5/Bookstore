using DddBookstore.Improved.Application;
using DddBookstore.Improved.Domain;

namespace DddBookstore.Improved.Infrastructure;

/// <summary>
/// Just logs events. A real project could send an email or notify the warehouse here.
/// </summary>
public sealed class ConsoleDomainEventPublisher : IDomainEventPublisher
{
    public void Publish(IDomainEvent domainEvent) =>
        Console.WriteLine($"[Domain Event] {domainEvent}");
}
