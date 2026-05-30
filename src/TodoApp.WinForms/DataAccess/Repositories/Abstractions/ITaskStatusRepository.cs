using TodoApp.WinForms.Models;

namespace TodoApp.WinForms.DataAccess.Abstractions
{
    public interface ITaskStatusRepository : ICrudRepository<TaskStatusEntity>
    {
    }
}
