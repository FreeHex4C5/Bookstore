using DddBookstore.Improved.Application;
using DddBookstore.Improved.Domain;

namespace DddBookstore.Web;

/// <summary>
/// WITH DDD: no rules here at all. It only calls the application service
/// and turns domain errors into a response for the UI.
/// </summary>
public sealed class DddOrderFacade : IOrderFacade
{
    private readonly OrderAppService _orders;

    public DddOrderFacade(OrderAppService orders) => _orders = orders;

    public OrderResponse Create(decimal unitPrice, int quantity) =>
        Execute(() => _orders.Create(unitPrice, quantity), Messages.Created);

    public OrderResponse Pay(Guid orderId) => Execute(() => _orders.Pay(orderId), Messages.Paid);

    public OrderResponse Ship(Guid orderId) => Execute(() => _orders.Ship(orderId), Messages.Shipped);

    public OrderResponse Cancel(Guid orderId) => Execute(() => _orders.Cancel(orderId), Messages.Cancelled);

    private static OrderResponse Execute(Func<OrderDto> useCase, string successMessage)
    {
        try
        {
            var order = useCase();
            return OrderResponse.Ok(successMessage,
                new OrderView(order.Id, order.UnitPrice, order.Quantity, order.Total, order.Status));
        }
        catch (DomainException ex)
        {
            return OrderResponse.Fail(ex.Message);
        }
        catch (OrderNotFoundException)
        {
            return OrderResponse.Fail(Messages.NotFound);
        }
    }
}
