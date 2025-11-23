using Northwind.Odata.Api.Data;
using Shared.Models;

public class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext _context;

    public OrderRepository(NorthwindContext context)
    {
        _context = context;
    }

    public IQueryable<Order> GetAll() => _context.Orders;

    public IQueryable<Order> GetById(int orderId) =>
        _context.Orders.Where(x => x.OrderId == orderId);

    public void Add(Order order) => _context.Orders.Add(order);

    public void Update(Order order)
    {
        var original = _context.Orders.Find(order.OrderId);
        if (original != null)
        {
            _context.Entry(original).CurrentValues.SetValues(order);
        }
    }

    public void Patch(Order order, Action<Order> patchAction)
    {
        patchAction(order);
    }

    public void Remove(Order order) => _context.Orders.Remove(order);

    public Order? Find(int orderId) => _context.Orders.Find(orderId);

    public void SaveChanges() => _context.SaveChanges();
}
