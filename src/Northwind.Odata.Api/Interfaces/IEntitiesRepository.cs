using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Models;
        #region Employee
public interface IEmployeeRepository
{
    Task<IQueryable<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IQueryable<Employee>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Employee entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(Employee entity, CancellationToken cancellationToken = default);
    Task<Employee?> FindAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
        #endregion Employee



