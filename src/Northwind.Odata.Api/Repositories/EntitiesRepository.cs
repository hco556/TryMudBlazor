using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Odata.Api.Data;
using Shared.Models;
        #region Employee

public class EmployeeRepository : IEmployeeRepository
{
    private readonly NorthwindContext _context;

    public EmployeeRepository(NorthwindContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Task.FromResult(_context.Set<Employee>());

    public async Task<IQueryable<Employee>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await Task.FromResult(_context.Set<Employee>().Where(x => x.EmployeeId == id));

    public async Task AddAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<Employee>().AddAsync(entity, cancellationToken);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        var original = await _context.Set<Employee>().FindAsync(new object[] { entity.EmployeeId }, cancellationToken);
        if (original != null)
        {
            _context.Entry(original).CurrentValues.SetValues(entity);
            await SaveChangesAsync();
        }
    }

    public async Task RemoveAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Employee>().Remove(entity);
        await SaveChangesAsync();
        await Task.CompletedTask;
    }

    public async Task<Employee?> FindAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Employee>().FindAsync(new object[] { id }, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}

        #endregion Employee

      
