using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.WinForms.Models;

namespace TodoApp.WinForms.Services.Abstractions
{
    public interface ITaskService
    {
        Task CreateTaskAsync(TodoTaskEntity task);
        Task<IEnumerable<TodoTaskEntity>> GetAllTasksAsync();
        Task DeleteTaskAsync(int id);
        Task UpdateTaskAsync(TodoTaskEntity task);
    }
}