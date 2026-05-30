using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services.Implementations;
using Xunit;

namespace TodoApp.Tests.Unit.Services
{
    public sealed class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepoMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _taskService = new TaskService(_taskRepoMock.Object);
        }

        [Fact]
        public async Task CreateTask_ShouldThrowException_WhenTitleIsEmpty()
        {
            // Arrange
            var invalidTask = new TodoTaskEntity { Name = "", PriorityId = 1, StatusId = 1 };

            // Act
            Func<Task> act = async () => await _taskService.CreateTaskAsync(invalidTask).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*The task name cannot be empty*")
                .ConfigureAwait(false);

            // Check that the repository was NOT called.
            _taskRepoMock.Verify(r => r.CreateAsync(It.IsAny<TodoTaskEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateTask_ShouldThrowException_WhenDueDateIsInPast()
        {
            // Arrange
            var invalidTask = new TodoTaskEntity
            {
                Name = "Correct name",
                DueDate = DateTime.Today.AddDays(-1),
                PriorityId = 1,
                StatusId = 1
            };

            // Act
            Func<Task> act = async () => await _taskService.CreateTaskAsync(invalidTask).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*The due date cannot be in the past*")
                .ConfigureAwait(false);

            _taskRepoMock.Verify(r => r.CreateAsync(It.IsAny<TodoTaskEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateTask_ShouldCallRepository_WhenTaskIsValid()
        {
            // Arrange
            var validTask = new TodoTaskEntity
            {
                Name = "Buy milk",
                DueDate = DateTime.Today.AddDays(1),
                PriorityId = 1,
                StatusId = 1
            };

            // Act
            await _taskService.CreateTaskAsync(validTask).ConfigureAwait(false);

            // Assert
            _taskRepoMock.Verify(r => r.CreateAsync(validTask), Times.Once);
        }

        [Fact]
        public async Task CreateTask_ShouldThrowException_WhenTitleExceeds150Characters()
        {
            // Arrange
            var fixture = new Fixture();
            var invalidTask = fixture.Create<TodoTaskEntity>();
            invalidTask.Name = new string('T', 151); // Max length is 150

            // Act
            Func<Task> act = async () => await _taskService.CreateTaskAsync(invalidTask).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().ConfigureAwait(false);
            _taskRepoMock.Verify(r => r.CreateAsync(It.IsAny<TodoTaskEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateTask_ShouldThrowException_WhenTaskIsNull()
        {
            // Act
            Func<Task> act = async () => await _taskService.CreateTaskAsync(null).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }
    }
}
