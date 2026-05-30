using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp.WinForms.DataAccess.Abstractions
{
    public interface ICrudRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
