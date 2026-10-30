using DddBookstore.Improved.Application;
using DddBookstore.Improved.Infrastructure;
using DddBookstore.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5080");
builder.Logging.SetMinimumLevel(LogLevel.Warning); // keep the console clean so domain events stand out
var app = builder.Build();

var versions = new Dictionary<string, IOrderFacade>
{
    ["before"] = new BeforeOrderFacade(),
    ["ddd"] = new DddOrderFacade(
        new OrderAppService(new InMemoryOrderRepository(), new ConsoleDomainEventPublisher())),
};

app.MapGet("/", () =>
    Results.File(Path.Combine(AppContext.BaseDirectory, "ui", "index.html"), "text/html; charset=utf-8"));

var orders = app.MapGroup("/api/{version}/orders");
orders.MapPost("/", (string version, CreateOrderRequest request) =>
    Use(version, facade => facade.Create(request.UnitPrice, request.Quantity)));
orders.MapPost("/{id:guid}/pay", (string version, Guid id) => Use(version, facade => facade.Pay(id)));
orders.MapPost("/{id:guid}/ship", (string version, Guid id) => Use(version, facade => facade.Ship(id)));
orders.MapPost("/{id:guid}/cancel", (string version, Guid id) => Use(version, facade => facade.Cancel(id)));

Console.WriteLine("UI: http://localhost:5080");
app.Run();

IResult Use(string version, Func<IOrderFacade, OrderResponse> action) =>
    versions.TryGetValue(version, out var facade) ? Results.Ok(action(facade)) : Results.NotFound();

record CreateOrderRequest(decimal UnitPrice, int Quantity);
