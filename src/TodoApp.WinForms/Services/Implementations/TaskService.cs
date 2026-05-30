using System.Collections.Generic;
using System;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services.Abstractions;
using System.Threading.Tasks;

namespace TodoApp.WinForms.Services.Implementations
{
    public sealed class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        }

        public Task<IEnumerable<TodoTaskEntity>> GetAllTasksAsync()
        {
            return _taskRepository.GetAllAsync();
        }

        public Task CreateTaskAsync(TodoTaskEntity task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            // Validate name.
            if (string.IsNullOrWhiteSpace(task.Name))
                throw new ArgumentException("The task name cannot be empty.");
            if (task.Name.Length > 150)
                throw new ArgumentException("The task title must not exceed 150 characters.");

            // The due date must not be in the past.
            if (task.DueDate.HasValue && task.DueDate.Value.Date < DateTime.Today)
            {
                throw new ArgumentException("The due date cannot be in the past.");
            }

            return _taskRepository.CreateAsync(task);
        }

        public Task DeleteTaskAsync(int id)
        {
            return _taskRepository.DeleteAsync(id);
        }

        public Task UpdateTaskAsync(TodoTaskEntity task)
        {
            return _taskRepository.UpdateAsync(task);
        }
    }
}
