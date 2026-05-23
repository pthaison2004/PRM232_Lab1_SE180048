using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lms.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetQueryable();
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
