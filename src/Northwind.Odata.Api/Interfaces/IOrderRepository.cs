using Shared.Models;

public interface IOrderRepository
{
    IQueryable<Order> GetAll();
    IQueryable<Order> GetById(int orderId);
    void Add(Order order);
    void Update(Order order);
    void Patch(Order order, Action<Order> patchAction);
    void Remove(Order order);
    Order? Find(int orderId);
    void SaveChanges();
}