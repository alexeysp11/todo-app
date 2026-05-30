using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.WinForms.Models;

namespace TodoApp.WinForms.DataAccess.Abstractions
{
    public interface ITaskRepository : ICrudRepository<TodoTaskEntity>
    {
        Task<IEnumerable<TodoTaskEntity>> GetByCategoryIdAsync(int categoryId);
        Task UpdateStatusAsync(int id, int newStatusId);
    }
}
